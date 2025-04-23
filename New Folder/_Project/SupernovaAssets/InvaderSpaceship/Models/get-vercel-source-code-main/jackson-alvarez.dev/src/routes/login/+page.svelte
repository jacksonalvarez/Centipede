<script>
import { goto } from '$app/navigation';  let email = '';
  let password = '';
  let errorMessage = '';

  // Function to handle form submission
  async function loginUser(event) {
    event.preventDefault();

    const response = await fetch('/api/auth/login', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ email, password }),
    });

    const data = await response.json();

    if (response.ok) {
      // Store the JWT token (in localStorage for example)
      localStorage.setItem('token', data.token);

      // Navigate to the dashboard or home page using goto
      goto('/Dashboard'); // You can change this route to wherever you want to redirect
    } else {
      errorMessage = data.message || 'Login failed';
    }
  }
</script>

<div class="login-container">
  <h1>Login</h1>
  <form on:submit={loginUser}>
    <div>
      <label for="email">Email</label>
      <input type="email" id="email" bind:value={email} required />
    </div>
    <div>
      <label for="password">Password</label>
      <input type="password" id="password" bind:value={password} required />
    </div>
    <button type="submit">Login</button>
  </form>

  {#if errorMessage}
    <div class="error">{errorMessage}</div>
  {/if}
</div>

<style>
  .login-container {
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
