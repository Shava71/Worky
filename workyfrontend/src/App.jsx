import './App.css'
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";import {Box, Button, Container} from "@mui/material";
import HomeIndex from "./Pages/HomeIndex.jsx";
import Company from "./Pages/Company.jsx";
import Worker from "./Pages/Worker.jsx";
import HeaderBar from "./Components/HeaderBar.jsx";
import LoginForm from "./Pages/Login.jsx";
import {useState} from "react";
import CompanyProfile from "./Pages/Profiles/CompanyProfile.jsx";
import Tariffs from "./Pages/Tariffs.jsx";
import CreateVacancy from "./Pages/Create/CreateVacancy.jsx";
import MyVacancy from "./Pages/MyVacancy.jsx";
import ResumesPage from "./Pages/Resumes.jsx";
import Resumes from "./Pages/Resumes.jsx";
import ResumeDetailsPage from "./Pages/Details/ResumeDetails.jsx";
import CompanyFeedbackPage from "./Pages/Feedback/CompanyFeedbacks.jsx";
import CompanyRegister from "./Pages/Register/CompanyRegister.jsx";
function App() {

    const [userRole, setUserRole] = useState(() => {
        const token = localStorage.getItem('jwt');
        if (!token) return null;

        try {
            const payload = JSON.parse(atob(token.split('.')[1]));
            return payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || null;
        } catch {
            return null;
        }
    });

    const [companyProfile, setCompanyProfile] = useState(null);

  return (
      <div>
          <Router>
              <HeaderBar userRole={userRole} setUserRole={setUserRole} setCompanyProfile={setCompanyProfile} />
              <Routes>
                  <Route path="/" element={<HomeIndex/>}/>

                  <Route path="/Company/Resumes" element={<ResumesPage/>}/>
                  <Route path="/Company/Resumes/Info/:resumeId" element={<ResumeDetailsPage />} />
                  <Route path="/Company/Feedbacks" element={<CompanyFeedbackPage/>} />

                  <Route path="/CompanyRegister" element={<CompanyRegister/>}/>
                  <Route path="/WorkerRegister" element={<Worker/>}/>
                  <Route path="/Login" element={<LoginForm setUserRole={setUserRole}/>}/>

                  <Route path="/Company/Profile" element={<CompanyProfile company={companyProfile?.company} deals={companyProfile?.deals} />} />
                  <Route path="/Tariffs" element={<Tariffs/>}/>

                  <Route path="/CreateVacancy" element={<CreateVacancy />}/>
                  <Route path="/MyVacancy" element={<MyVacancy />}/>
              </Routes>
          </Router>
      </div>
  )
}

export default App
