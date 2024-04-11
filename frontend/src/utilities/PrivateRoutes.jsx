import { Outlet, useNavigate } from "react-router-dom";
import { useState, useEffect } from "react";

const PrivateRoutes = () => {
  const navigate = useNavigate();

  // Initialize state for the token
  const [token, setToken] = useState(localStorage.getItem("token"));

  // Use useEffect to watch for changes in localStorage
  useEffect(() => {
    const handleStorageChange = (e) => {
      if (e.key === "token") {
        // Token has changed in localStorage, update the state
        setToken(localStorage.getItem("token"));
      }
    };

    // Add event listener for changes in localStorage
    window.addEventListener("storage", handleStorageChange);

    // Clean up the event listener when the component unmounts
    return () => {
      window.removeEventListener("storage", handleStorageChange);
    };
  }, []);

  useEffect(() => {
    // Check if the token exists in localStorage
    const tokenInLocalStorage = localStorage.getItem("token");

    // If the token doesn't exist, navigate to the /login route
    if (!tokenInLocalStorage) {
      navigate("/login");
    }
  }, [token, navigate]);

  return token ? <Outlet /> : null;
};

export default PrivateRoutes;
