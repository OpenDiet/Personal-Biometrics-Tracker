import { Container, Typography, Paper, Box, Button } from "@mui/material";
import FitnessCenterIcon from "@mui/icons-material/FitnessCenter";

const Layout = () => {
  return (
    <Container maxWidth="sm">
      <Paper elevation={3}>
        <Box p={3} textAlign="center">
          <Typography variant="h4" gutterBottom>
            Personal Biometrics Tracker
          </Typography>
          <Typography variant="body1" paragraph>
            Your personal health and fitness dashboard. Coming soon!
          </Typography>
          <Button
            variant="contained"
            color="primary"
            startIcon={<FitnessCenterIcon />}
          >
            Get Started
          </Button>
        </Box>
      </Paper>
    </Container>
  );
};

export default Layout;
