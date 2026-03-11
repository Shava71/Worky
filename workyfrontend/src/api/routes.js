export const API = {
    auth: {
        login: '/api/Auth/Login',
        register: '/api/Auth/Register',
        profile: (userId) => `/api/Auth/User/Profile?userId=${userId}`,
        uploadPhoto: (userId) => `api/Auth/profile-photo/upload/${userId}`,
        getPhoto: (userId) => `api/Auth/profile-photo/${userId}`,
        deletePhoto: (userId) => `api/Auth/profile-photo/${userId}`,
        updatePhoto: (userId) => `api/Auth/profile-photo/${userId}`,

    },

    worker: {
        resumesInfo: '/api/Worker/Resumes/Info',
        myResume: '/api/Worker/MyResume',
        createResume: '/api/Worker/CreateResume',
        updateResume: '/api/Worker/UpdateResume',
        deleteResume: '/api/Worker/DeleteResume',
        addResumeFilter: '/api/Worker/AddResumeFilter',
        deleteResumeFilter: '/api/Worker/DeleteResumeFilter',
        profile: '/api/Worker/GetProfile',
    },

    company: {
        vacancyInfo: '/api/v1/Company/Vacancies/Info',
        myVacancy: '/api/v1/Company/MyVacancy',
        createVacancy: '/api/v1/Company/CreateVacancy',
        updateVacancy: '/api/v1/Company/UpdateVacancy',
        deleteVacancy: '/api/v1/Company/DeleteVacancy',
        addVacancyFilter: '/api/v1/Company/AddVacancyFilter',
        deleteVacancyFilter: '/api/v1/Company/DeleteVacancyFilter',
        flyer: '/api/v1/Company/Flyer',
        profile: '/api/v1/Company/GetProfile',
        updateProfile: (userId) => `/api/v1/Company/update/${userId}`,
    },

    deal: {
        tariffs: '/api/v1/Deal/Tarrif',
        makeDeal: '/api/v1/Deal/MakeDeal',
    },

    feedback: {
        make: '/api/Feedback/MakeFeedback',
        delete: '/api/Feedback/DeleteFeedback',
        get: '/api/Feedback/GetFeedback',
        accept: '/api/Feedback/AcceptFeedback',
        reject: '/api/Feedback/RejectFeedback',
    },

    filter: {
        get: '/api/Filter/GetFilters',
        add: '/api/Filter/AddFilter',
        education: '/api/Education/GetEducation',
    },

    search: {
        resumes: '/api/resumes',
        vacancies: '/api/vacancies',
        click: '/api/search/click',
    },
};