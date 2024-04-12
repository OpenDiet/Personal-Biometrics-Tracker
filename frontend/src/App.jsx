import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import PrivateRoutes from "./utilities/PrivateRoutes";
import Login from "./components/login/Login";
import Registration from "./components/registration/Registration";
import "./index.css";

function App() {
  return (
    <Router>
      <Routes>
        {/* UNPROTECTED ROUTES */}
        <Route path="/login" element={<Login />} />
        <Route path="/registration" element={<Registration />} />

        {/* PROTECTED ROUTES */}
        <Route element={<PrivateRoutes />}>
          <Route path="/" index element={<div>hello world</div>} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
