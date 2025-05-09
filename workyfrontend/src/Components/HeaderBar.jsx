import React, { useEffect, useState } from 'react';
import {
    AppBar, Box, Toolbar, IconButton, Typography, Menu,
    Container, Button, Tooltip, MenuItem
} from '@mui/material';
import MenuIcon from '@mui/icons-material/Menu';
import BusinessIcon from '@mui/icons-material/Business';
import PersonIcon from '@mui/icons-material/Person';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';

const pages = ['Соискателям', 'Компаниям'];

export default function HeaderBar({ userRole, setUserRole, setCompanyProfile }) {
    const [anchorElNav, setAnchorElNav] = useState(null);
    const [anchorElUser, setAnchorElUser] = useState(null);
    const [companyName, setCompanyName] = useState(null); // <--- Добавлено

    const navigate = useNavigate();

    const handleOpenNavMenu = (event) => setAnchorElNav(event.currentTarget);
    const handleOpenUserMenu = (event) => setAnchorElUser(event.currentTarget);
    const handleCloseNavMenu = () => setAnchorElNav(null);
    const handleCloseUserMenu = () => setAnchorElUser(null);

    const handleLogout = () => {
        localStorage.removeItem('jwt');
        localStorage.removeItem('role');
        localStorage.removeItem('userId');
        setUserRole(null);
        navigate('/Login');
    };

    const handleMenuClick = (setting) => {
        handleCloseUserMenu();
        switch (setting) {
            case 'Профиль':
                if (userRole === 'Company') navigate('/Company/Profile');
                break;
            case 'Отклики':
                navigate('/Company/Feedbacks');
                break;
            case 'Мои вакансии':
                navigate('/MyVacancy');
                break;
            case 'Выйти':
                handleLogout();
                break;
            default:
                break;
        }
    };

    const getMenuItems = () => {
        if (userRole === 'Company') return ['Профиль', 'Отклики', 'Мои вакансии', 'Выйти'];
        if (userRole) return ['Профиль', 'Выйти'];
        return [];
    };

    useEffect(() => {
        const fetchCompanyProfile = async () => {
            try {
                const token = localStorage.getItem('jwt');
                if (!token || userRole !== 'Company') return;

                const response = await axios.get('https://localhost:7106/api/v1/Company/GetProfile', {
                    headers: { Authorization: `Bearer ${token}` }
                });

                const profile = response.data;
                setCompanyProfile(profile);
                setCompanyName(profile.company?.name); // <--- Сохраняем имя
            } catch (error) {
                console.error('Ошибка при получении профиля компании:', error);
                handleLogout();
            }
        };

        fetchCompanyProfile();
    }, [userRole]);

    return (
        <AppBar position="fixed" sx={{ bgcolor: '#2e2e2e' }}>
            <Container maxWidth="xl">
                <Toolbar disableGutters>
                    <Typography
                        variant="h6"
                        noWrap
                        component="a"
                        href="/"
                        sx={{
                            mr: 2,
                            display: { xs: 'none', md: 'flex' },
                            fontWeight: 700,
                            letterSpacing: '.2rem',
                            color: 'white',
                            textDecoration: 'none',
                        }}
                    >
                        Worky
                    </Typography>

                    <Box sx={{ flexGrow: 1, display: { xs: 'flex', md: 'none' } }}>
                        <IconButton
                            size="large"
                            onClick={handleOpenNavMenu}
                            color="inherit"
                        >
                            <MenuIcon />
                        </IconButton>
                        <Menu
                            anchorEl={anchorElNav}
                            open={Boolean(anchorElNav)}
                            onClose={handleCloseNavMenu}
                        >
                            {pages.map((page) => (
                                // <MenuItem key={page} onClick={handleCloseNavMenu}>
                                <MenuItem key={page} onClick={ () =>{
                                            handleCloseNavMenu();
                                            console.log(page);
                                            if (page === 'Компаниям') {
                                                navigate("/Company/Resumes");
                                            } else {
                                                navigate("/Worker/Vacancies");
                                            }
                                        }
                                     }>
                                    <Typography textAlign="center">{page}</Typography>
                                </MenuItem>
                            ))}
                        </Menu>
                    </Box>

                    <Typography
                        variant="h5"
                        noWrap
                        component="a"
                        href="/"
                        sx={{
                            mr: 2,
                            display: { xs: 'flex', md: 'none' },
                            flexGrow: 1,
                            fontWeight: 700,
                            letterSpacing: '.2rem',
                            color: 'white',
                            textDecoration: 'none',
                        }}
                    >
                        Worky
                    </Typography>

                    <Box sx={{ flexGrow: 1, display: { xs: 'none', md: 'flex' } }}>
                        {pages.map((page) => (
                            <Button
                                key={page}
                                onClick={() => {
                                    handleCloseNavMenu();
                                    console.log(page);
                                    if (page === 'Компаниям') {
                                        navigate("/Company/Resumes");
                                    } else {
                                        navigate("/Worker/Vacancies");
                                    }
                                }}
                                sx={{ my: 2, color: 'white', display: 'block' }}
                            >
                                {page}
                            </Button>
                        ))}
                    </Box>

                    {userRole ? (
                        <Box sx={{ flexGrow: 0, display: 'flex', alignItems: 'center', gap: 1 }}>
                            {userRole === 'Company' ? (
                                <Button color="inherit" onClick={() => navigate('/CreateVacancy')}>
                                    Выложить вакансию
                                </Button> ) : (

                                <Button color="inherit" onClick={() => navigate('/CreateResume')}>
                                    Выложить резюме
                                </Button>
                            )}

                            <Tooltip title="Открыть меню">
                                <IconButton onClick={handleOpenUserMenu} sx={{ p: 0 }}>
                                    {userRole === 'Company' ? (
                                        <BusinessIcon sx={{ color: 'white' }} />
                                    ) : (
                                        <PersonIcon sx={{ color: 'white' }} />
                                    )}
                                </IconButton>
                            </Tooltip>
                            {userRole === 'Company' && companyName && (
                                <Typography variant="body1" sx={{ color: 'white', mr: 1 }}>
                                    {companyName}
                                </Typography>
                            )}
                            <Menu
                                sx={{ mt: '45px' }}
                                anchorEl={anchorElUser}
                                open={Boolean(anchorElUser)}
                                onClose={handleCloseUserMenu}
                            >
                                {getMenuItems().map((setting) => (
                                    <MenuItem key={setting} onClick={() => handleMenuClick(setting)}>
                                        <Typography textAlign="center">{setting}</Typography>
                                    </MenuItem>
                                ))}
                            </Menu>
                        </Box>
                    ) : (
                        <Box sx={{ display: 'flex', gap: 2 }}>
                            <Button color="inherit" onClick={() => navigate('/Login')}>
                                Войти
                            </Button>
                            <Button color="inherit" onClick={() => navigate('/')}>
                                Зарегистрироваться
                            </Button>
                        </Box>
                    )}
                </Toolbar>
            </Container>
        </AppBar>
    );
}