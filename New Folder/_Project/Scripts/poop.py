import numpy as np
import pandas as pd
from pgmpy.models import BayesianNetwork
from pgmpy.factors.discrete import TabularCPD
from pgmpy.inference import VariableElimination
from pgmpy.sampling import GibbsSampling
import warnings


warnings.filterwarnings("ignore", category=UserWarning, module='pgmpy')


def adjacency_to_edges(adj_matrix, node_names):
    """Converts an adjacency matrix into edge list."""
    edges = []
    for i, parent in enumerate(node_names):
        for j, child in enumerate(node_names):
            if adj_matrix[i, j] == 1:
                edges.append((parent, child))
    return edges

node_names = ['Burglary', 'Earthquake', 'Alarm', 'JohnCalls', 'MaryCalls']
adj_matrix = np.array([
    [0, 0, 1, 0, 0],  
    [0, 0, 1, 0, 0],  
    [0, 0, 0, 1, 1],  
    [0, 0, 0, 0, 0],  
    [0, 0, 0, 0, 0]
])

edges = adjacency_to_edges(adj_matrix, node_names)
model = BayesianNetwork(edges)

cpd_burglary = TabularCPD('Burglary', 2, [[0.01], [0.99]])
cpd_earthquake = TabularCPD('Earthquake', 2, [[0.02], [0.98]])
cpd_alarm = TabularCPD('Alarm', 2, 
                       values=[[0.95, 0.94, 0.29, 0.001], 
                               [0.05, 0.06, 0.71, 0.999]], 
                       evidence=['Burglary', 'Earthquake'], evidence_card=[2, 2])
cpd_john_calls = TabularCPD('JohnCalls', 2, [[0.9, 0.05], [0.1, 0.95]], evidence=['Alarm'], evidence_card=[2])
cpd_mary_calls = TabularCPD('MaryCalls', 2, [[0.7, 0.01], [0.3, 0.99]], evidence=['Alarm'], evidence_card=[2])

model.add_cpds(cpd_burglary, cpd_earthquake, cpd_alarm, cpd_john_calls, cpd_mary_calls)
assert model.check_model()

# -------------------------------
# 2. Exact Inference using Variable Elimination
# -------------------------------
def variable_elimination_inference(model, query_vars, evidence):
    """Performs variable elimination to compute exact probabilities."""
    inference = VariableElimination(model)
    result = inference.query(variables=query_vars, evidence=evidence)
    return result

result = variable_elimination_inference(model, ['Earthquake'], {'JohnCalls': 0, 'MaryCalls': 0})
print("Exact Probability P(Earthquake | JohnCalls=0, MaryCalls=0):\n", result)

# -------------------------------
# 3. Approximate Inference using Gibbs Sampling
# -------------------------------
def gibbs_sampling_inference(model, evidence, num_samples=10000, burn_in=1000):
    """Performs Gibbs sampling to approximate the posterior distribution."""
    gibbs = GibbsSampling(model)

    samples = gibbs.sample(size=num_samples)

    samples = samples[burn_in:]

    return samples

samples = gibbs_sampling_inference(model, {'JohnCalls': 0, 'MaryCalls': 0}, num_samples=10000, burn_in=1000)

approx_prob = np.mean(samples['Earthquake'] == 0)

print("Approximate Probability P(Earthquake | JohnCalls=0, MaryCalls=0) using Gibbs Sampling:", approx_prob)

# -------------------------------
# 4. Test D-Separation
# -------------------------------
def test_d_separation(model, X, Y, Z):
    """Checks if X and Y are d-separated given Z."""
    result = model.is_dconnected(X, Y, observed=Z)
    print(f"D-Separation: {X} and {Y} are {'NOT ' if result else ''}d-separated given {Z}")

test_d_separation(model, 'Burglary', 'Earthquake', ['Alarm'])
test_d_separation(model, 'JohnCalls', 'MaryCalls', ['Alarm'])
