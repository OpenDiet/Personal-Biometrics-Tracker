import Container from "@mui/material/Container";
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Box from "@mui/material/Box";
import Paper from "@mui/material/Paper";
import { ThemeProvider } from "@mui/material/styles";
import theme from "../../theme";

const Layout = ({ children }) => {
  return (
    <ThemeProvider theme={theme}>
      <Container maxWidth="lg">
        <AppBar position="static">
          <Toolbar>
            <Typography variant="h6">Personal Biometrics Tracker</Typography>
          </Toolbar>
        </AppBar>
        <Box my={4}>
          <Paper elevation={3}>
            <Box p={3}>
              <main>{children}</main>
            </Box>
          </Paper>
        </Box>
        <footer>
          <Typography variant="body2" color="textSecondary" align="center">
            &copy; {new Date().getFullYear()} OpenDiet.io. All Rights Reserved.
            Open source project licensed under AGPL v3.
          </Typography>
        </footer>
      </Container>
    </ThemeProvider>
  );
};

export default Layout;
