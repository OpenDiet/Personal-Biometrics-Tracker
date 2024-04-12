import { useState } from "react";
import { Link as RouterLink } from "react-router-dom";
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
import { Email, HowToReg, Person } from "@mui/icons-material";
import PasswordIcon from "@mui/icons-material/Password";

const Registration = () => {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [error, setError] = useState("");

  const handleSubmit = (event) => {
    event.preventDefault();
    setError("");

    // TODO: Implement validation
    if (!username || !password || !email) {
      setError("Username, email, and password are required.");
      return;
    }

    // TODO: Implement registration logic
    console.log("Registration Submitted", { username, email, password });

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
            Register as a new user
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
              id="email"
              label="Email"
              name="email"
              autoComplete="email"
              InputProps={{
                startAdornment: (
                  <InputAdornment position="start">
                    <Email />
                  </InputAdornment>
                ),
              }}
              value={email}
              onChange={(e) => setEmail(e.target.value)}
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
                  control={<Checkbox />}
                  label="I agree to Terms and Conditions"
                />
              </FormGroup>
            </Box>
            <Button
              type="submit"
              fullWidth
              variant="contained"
              startIcon={<HowToReg />}
              sx={{ marginTop: 1 }}
            >
              Register
            </Button>
            <Typography mt={2}>
              Already have an account?{" "}
              <Link component={RouterLink} to="/login">
                Login to your account
              </Link>
              !
            </Typography>
          </Box>
        </Box>
      </Box>
    </Layout>
  );
};

export default Registration;
