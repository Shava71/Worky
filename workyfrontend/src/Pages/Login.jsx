import React, { useState } from 'react';
import axios from 'axios';
import {
    Button,
    TextField,
    Box,
    Typography,
    Container,
    Alert,
    Paper,
    InputAdornment,
    IconButton
} from '@mui/material';
import { useNavigate } from 'react-router-dom';
import Visibility from '@mui/icons-material/Visibility';
import VisibilityOff from '@mui/icons-material/VisibilityOff';
import LoginIcon from '@mui/icons-material/Login';
import api from '../api/myaxios.js';
import { API } from '../api/routes';


export default function LoginForm({setUserRole}) {
    const [email, setEmail] = useState('');
    const [password, setPassword] = useState('');
    const [showPassword, setShowPassword] = useState(false);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    const handleLogin = async () => {
        setError(null);
        try {
            const response = await api.post(API.auth.login, {
                email,
                password,
            });

            const { token, role, id } = response.data;

            axios.defaults.headers.common['Authorization'] = `Bearer ${token}`;
            localStorage.setItem('jwt', token);
            localStorage.setItem('role', role);
            localStorage.setItem('userId', id);

            setUserRole(role);

            if (role.includes("Worker")) {
                navigate('/Worker/Vacancies');
            } else if (role.includes("Company")) {
                navigate('/Company/Resumes');
            } else {
                navigate('/');
            }

        } catch (err) {
            console.error('Login failed:', err);
            setError('Неверный email или пароль');
        }
    };

    return (
        <Container maxWidth="sm">
            <Paper
                elevation={6}
                sx={{
                    mt: 10,
                    p: 5,
                    borderRadius: 4,
                    display: 'flex',
                    flexDirection: 'column',
                    alignItems: 'center',
                    gap: 3,
                    backgroundColor: '#f9f9f9',
                }}
            >
                <Typography variant="h4" fontWeight="bold" color="primary" gutterBottom>
                    Вход в систему
                </Typography>

                {error && <Alert severity="error" data-testid="login-error" sx={{ width: '100%' }}>{error}</Alert>}

                <TextField
                    label="Email"
                    fullWidth
                    variant="outlined"
                    value={email}
                    onChange={(e) => setEmail(e.target.value)}
                    autoComplete="email"
                    inputProps={{ "data-testid": "email-input" }}
                />

                <TextField
                    label="Пароль"
                    fullWidth
                    variant="outlined"
                    type={showPassword ? 'text' : 'password'}
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                    autoComplete="current-password"
                    inputProps={{ "data-testid": "password-input" }}
                    InputProps={{
                        endAdornment: (
                            <InputAdornment position="end">
                                <IconButton
                                    onClick={() => setShowPassword(!showPassword)}
                                    edge="end"
                                >
                                    {showPassword ? <VisibilityOff /> : <Visibility />}
                                </IconButton>
                            </InputAdornment>
                        ),
                    }}
                />

                <Button
                    data-testid="login-button"
                    variant="contained"
                    color="primary"
                    size="large"
                    fullWidth
                    endIcon={<LoginIcon />}
                    onClick={handleLogin}
                    sx={{ textTransform: 'none', fontSize: '1.1rem' }}
                >
                    Войти
                </Button>
            </Paper>
        </Container>
    );
}