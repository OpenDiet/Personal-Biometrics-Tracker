import { useState } from "react";
import {
  Box,
  Button,
  Checkbox,
  FormControlLabel,
  FormGroup,
  InputAdornment,
  Link,
  TextField,
  Typography,
} from "@mui/material";
import Layout from "../layout/Layout";
import { Person } from "@mui/icons-material";
import PasswordIcon from "@mui/icons-material/Password";
import LoginIcon from "@mui/icons-material/Login";

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
          width: "100%",
          display: "flex",
          flexDirection: "column",
          alignItems: "center",
        }}
      >
        <Box
          sx={{
            marginTop: 8,
            marginBottom: 8,
            display: "flex",
            flexDirection: "column",
            width: "100%",
            maxWidth: {
              xs: "100%",
              sm: "50%",
            },
          }}
        >
          <Typography component="h1" variant="h5">
            Login to your account
          </Typography>
          {error && (
            <Typography color="error" mt={1} mb={1}>
              {error}
            </Typography>
          )}
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
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Person />
                  </InputAdornment>
                ),
              }}
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
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <PasswordIcon />
                  </InputAdornment>
                ),
              }}
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
            <Box
              sx={{
                display: "flex",
                flexDirection: "row",
                alignItems: "center",
                justifyContent: "space-between",
              }}
            >
              <FormGroup>
                <FormControlLabel
                  control={<Checkbox defaultChecked />}
                  label="Stay logged in"
                />
              </FormGroup>
              <Link href="#">Forgot password?</Link>
            </Box>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              startIcon={<LoginIcon />}
              sx={{ marginTop: 1 }}
            >
              Sign In
            </Button>
            <Typography mt={2}>
              Don&apos;t have an account yet? <Link href="#">Sign up</Link>!
            </Typography>
          </Box>
        </Box>
      </Box>
    </Layout>
  );
};

export default Login;
