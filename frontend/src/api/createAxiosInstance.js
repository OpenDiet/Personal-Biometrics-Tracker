import axios from "axios";

// Generate an Axios instance with a provided token
const createAxiosInstance = () => {
  // Get the token from storage
  const token = localStorage.getItem("token");

  let baseURL = import.meta.env.VITE_API_URL + "/";

  const axiosInstance = axios.create({
    baseURL,
    headers: {
      Authorization: `Bearer ${token}`, // Use the provided token in the authorization header
      Accept: "application/json",
    },
  });

  axiosInstance.interceptors.response.use(
    (response) => {
      return response;
    },
    (error) => {
      if (error.response && error.response.status === 401) {
        // The user is not authenticated
        localStorage.removeItem("token");
      }
      return Promise.reject(error);
    }
  );

  return axiosInstance;
};

export default createAxiosInstance;
