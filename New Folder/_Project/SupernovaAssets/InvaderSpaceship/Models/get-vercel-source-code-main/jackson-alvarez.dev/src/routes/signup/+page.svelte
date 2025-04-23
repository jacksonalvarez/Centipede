<script>
import { goto } from '$app/navigation';  let email = '';
  let password = '';
  let confirmPassword = '';
  let errorMessage = '';

  // Function to handle form submission
  async function signupUser(event) {
    event.preventDefault();

    if (password !== confirmPassword) {
      errorMessage = 'Passwords do not match';
      return;
    }

    const response = await fetch('/api/auth/signup', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
    });

    const data = await response.json();

    if (response.ok) {
      // Optionally, automatically log in after signup or redirect
      goto('/login'); // Redirect to login page after successful signup
    } else {
      errorMessage = data.message || 'Signup failed';
    }
  }
</script>

<div class="signup-container">
  <h1>Signup</h1>
  <form on:submit={signupUser}>
    <div>
      <label for="email">Email</label>
      <input type="email" id="email" bind:value={email} required />
    </div>
    <div>
      <label for="password">Password</label>
      <input type="password" id="password" bind:value={password} required />
    </div>
    <div>
      <label for="confirmPassword">Confirm Password</label>
      <input type="password" id="confirmPassword" bind:value={confirmPassword} required />
    </div>
    <button type="submit">Sign Up</button>
  </form>

  {#if errorMessage}
    <div class="error">{errorMessage}</div>
  {/if}
</div>

<style>
  .signup-container {
    width: 300px;
    margin: 0 auto;
    padding: 20px;
    text-align: center;
  }

  input {
    width: 100%;
    padding: 8px;
    margin-bottom: 10px;
    border-radius: 5px;
  }

  button {
    width: 100%;
    padding: 10px;
    border-radius: 5px;
    background-color: #007bff;
    color: white;
  }

  .error {
    color: red;
    margin-top: 10px;
  }
</style>
