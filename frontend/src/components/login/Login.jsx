import React, { useState } from "react";
import { Box, Button, TextField, Typography, Container } from "@mui/material";
import Layout from "../layout/Layout";

const Login = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (event) => {
    event.preventDefault();
    setError("");

    // TODO: Implement validation
    if (!username || !password) {
      setError("Please enter both username and password");
      return;
    }

    // TODO: Implement login logic
    console.log("Login Submitted", { username, password });

    // Reset form fields
    setUsername("");
    setPassword("");
  };

  return (
    <Layout>
      <Box
        sx={{
          marginTop: 8,
          marginBottom: 8,
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
        }}
      >
        <Typography component="h1" variant="h5">
          Login
        </Typography>
        {error && <Typography color="error">{error}</Typography>}
        <Box component="form" onSubmit={handleSubmit} noValidate>
          <TextField
            margin="normal"
            required
            fullWidth
            id="username"
            label="Username"
            name="username"
            autoComplete="username"
            autoFocus
            value={username}
            onChange={(e) => setUsername(e.target.value)}
          />
          <TextField
            margin="normal"
            required
            fullWidth
            name="password"
            label="Password"
            type="password"
            id="password"
            autoComplete="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
          />
          <Button type="submit" fullWidth variant="contained">
            Sign In
          </Button>
        </Box>
      </Box>
    </Layout>
  );
};

export default Login;
