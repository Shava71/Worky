import React, { useState } from 'react';
import {
    Container,
    Paper,
    Typography,
    Box,
    Stack,
    TextField,
    Button,
    Divider,
    Snackbar,
    Alert,
} from '@mui/material';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import dayjs from 'dayjs';

import api from "../../api/axios.js";
import {API} from "../../api/routes.js";

export default function WorkerRegister() {
    const [formData, setFormData] = useState({
        userName: '',
        email: '',
        phoneNumber: '',
        passwordHash: '',
        role: 'Worker',
        first_name: '',
        second_name: '',
        surname: '',
        birthday: null,
    });

    const [errors, setErrors] = useState({});
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
        if (value.trim()) {
            setErrors(prev => ({ ...prev, [name]: '' }));
        }
    };

    const handleDateChange = (date) => {
        setFormData(prev => ({
            ...prev,
            birthday: date ? dayjs(date).format('YYYY-MM-DD') : null,
        }));
    };

    const validateForm = () => {
        const newErrors = {};
        let isValid = true;

        if (!formData.userName.trim()) {
            newErrors.userName = 'Логин обязателен';
            isValid = false;
        }

        if (!formData.email.trim()) {
            newErrors.email = 'Email обязателен';
            isValid = false;
        } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
            newErrors.email = 'Некорректный email';
            isValid = false;
        }

        if (!formData.passwordHash) {
            newErrors.passwordHash = 'Пароль обязателен';
            isValid = false;
        }

        if (!formData.first_name.trim()) {
            newErrors.first_name = 'Имя обязательно';
            isValid = false;
        }

        if (!formData.second_name.trim()) {
            newErrors.second_name = 'Фамилия обязательна';
            isValid = false;
        }

        if (!formData.birthday) {
            newErrors.birthday = 'Укажите дату рождения';
            isValid = false;
        }

        setErrors(newErrors);
        return isValid;
    };

    const handleSubmit = async () => {
        if (!validateForm()) return;

        try {
            // const token = localStorage.getItem('jwt');
            // await axios.post('https://localhost:7106/api/v1/Authorization/Register', {
            //     ...formData,
            // }, {
            //     headers: {
            //         Authorization: `Bearer ${token}`,
            //         'Content-Type': 'application/json'
            //     }
            // });
            await api.post(API.auth.register, {
                ...formData
            })

            setSnackbar({
                open: true,
                message: 'Вы успешно зарегистрировались!',
                severity: 'success',
            });

            setTimeout(() => {
                window.location.href = '/Login';
            }, 1500);
        } catch (err) {
            console.error('Ошибка при регистрации:', err.response?.data || err.message);

            // Если это ответ от Identity
            if (err.response?.data && Array.isArray(err.response.data)) {
                const identityErrors = {};
                err.response.data.forEach(error => {
                    if (error.code === 'DuplicateUserName') {
                        identityErrors.userName = 'Этот логин уже занят';
                    } else if (error.code === 'DuplicateEmail') {
                        identityErrors.email = 'Этот email уже используется';
                    } else if (error.code === 'PasswordTooShort') {
                        identityErrors.passwordHash = 'Пароль слишком короткий';
                    } else if (error.code === 'InvalidPhoneNumber') {
                        identityErrors.phoneNumber = 'Некорректный номер телефона';
                    } else {
                        identityErrors.generic = error.description || 'Ошибка регистрации';
                    }
                });
                setErrors(identityErrors);
            } else {
                setErrors({ generic: 'Не удалось зарегистрироваться' });
            }

            setSnackbar({
                open: true,
                message: 'Не удалось зарегистрироваться',
                severity: 'error',
            });
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbar(prev => ({ ...prev, open: false }));
    };

    return (
        <Container maxWidth="md" sx={{ py: 6 }}>
            <Paper elevation={3} sx={{ p: 4, borderRadius: 3 }}>
                <Typography variant="h5" fontWeight="bold" gutterBottom align="center">
                    👤 Регистрация соискателя
                </Typography>
                <Divider sx={{ mb: 3 }} />

                <Stack spacing={2}>
                    {/* Основные поля */}
                    <TextField
                        label="Логин"
                        name="userName"
                        value={formData.userName}
                        onChange={handleChange}
                        fullWidth
                        error={!!errors.userName}
                        helperText={errors.userName}
                    />
                    <TextField
                        label="Email"
                        name="email"
                        value={formData.email}
                        onChange={handleChange}
                        fullWidth
                        error={!!errors.email}
                        helperText={errors.email}
                    />
                    <TextField
                        label="Номер телефона"
                        name="phoneNumber"
                        value={formData.phoneNumber}
                        onChange={handleChange}
                        fullWidth
                        error={!!errors.phoneNumber}
                        helperText={errors.phoneNumber}
                    />
                    <TextField
                        label="Пароль"
                        name="passwordHash"
                        type="password"
                        value={formData.passwordHash}
                        onChange={handleChange}
                        fullWidth
                        error={!!errors.passwordHash}
                        helperText={errors.passwordHash}
                    />

                    {/* Личные данные */}
                    <TextField
                        label="Имя"
                        name="first_name"
                        value={formData.first_name}
                        onChange={handleChange}
                        fullWidth
                        error={!!errors.first_name}
                        helperText={errors.first_name}
                    />
                    <TextField
                        label="Фамилия"
                        name="second_name"
                        value={formData.second_name}
                        onChange={handleChange}
                        fullWidth
                        error={!!errors.second_name}
                        helperText={errors.second_name}
                    />
                    <TextField
                        label="Отчество"
                        name="surname"
                        value={formData.surname}
                        onChange={handleChange}
                        fullWidth
                    />

                    {/* Дата рождения с DatePicker */}
                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                        <DatePicker
                            label="Дата рождения"
                            value={formData.birthday ? dayjs(formData.birthday) : null}
                            onChange={handleDateChange}
                            format="DD.MM.YYYY"
                            renderInput={(params) => (
                                <TextField {...params} fullWidth error={!!errors.birthday} helperText={errors.birthday} />
                            )}
                        />
                    </LocalizationProvider>

                    {/* Сообщение об общей ошибке */}
                    {errors.generic && (
                        <Alert severity="error" sx={{ mt: 2 }}>
                            {errors.generic}
                        </Alert>
                    )}

                    <Button
                        variant="contained"
                        color="primary"
                        onClick={handleSubmit}
                        fullWidth
                        sx={{ mt: 2 }}
                    >
                        Зарегистрироваться
                    </Button>
                </Stack>
            </Paper>

            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={handleCloseSnackbar}>
                <Alert severity={snackbar.severity}>{snackbar.message}</Alert>
            </Snackbar>
        </Container>
    );
}