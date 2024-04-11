import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import PrivateRoutes from "./utilities/PrivateRoutes";
import Login from "./components/login/Login";

function App() {
  return (
    <Router>
      <Routes>
        {/* UNPROTECTED ROUTES */}
        <Route path="/login" element={<Login />} />

        {/* PROTECTED ROUTES */}
        <Route element={<PrivateRoutes />}>
          <Route path="/" index element={<div>hello world</div>} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
