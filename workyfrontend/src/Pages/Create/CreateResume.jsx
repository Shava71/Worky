import React, { useEffect, useState } from 'react';
import {
    Box,
    Container,
    Typography,
    TextField,
    Button,
    Stack,
    Snackbar,
    Alert,
    Paper,
    Divider,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Chip,
    CircularProgress,
} from '@mui/material';
// import axios from 'axios';
import { useNavigate } from 'react-router-dom';
import api from '../../api/axios';
import { API } from '../../api/routes';

export default function CreateVacancy() {
    const [formData, setFormData] = useState({
        post: '',
        wantedSalary: 1,
        education_id: 1,
        experience: 0,
        skill: '',
        city: ''
    });
    const [educationList, setEducationList] = useState([]);
    const [loadingEducation, setLoadingEducation] = useState(true);
    const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
    const [filters, setFilters] = useState([]);
    const [selectedType, setSelectedType] = useState('');
    const [selectedDirections, setSelectedDirections] = useState([]);
    const [filteredDirections, setFilteredDirections] = useState([]);
    const navigate = useNavigate();

    // Загрузка образования
    useEffect(() => {
        const fetchEducation = async () => {
            try {
                // const response = await axios.get('https://localhost:7106/api/v1/GetInfo/Education');
                const response = await api.get(API.filter.education);
                setEducationList(response.data.education || []);
            } catch (error) {
                console.error('Ошибка загрузки образования:', error);
                setSnackbar({
                    open: true,
                    message: 'Не удалось загрузить список образования',
                    severity: 'error',
                });
            } finally {
                setLoadingEducation(false);
            }
        };
        fetchEducation();
    }, []);

    // Загрузка фильтров
    useEffect(() => {
        const fetchFilters = async () => {
            try {
                // const response = await axios.get('https://localhost:7106/api/v1/GetInfo/Filter');
                const response = await api.get(API.filter.get);
                setFilters(response.data || []);
            } catch (err) {
                console.error('Ошибка при загрузке фильтров:', err);
            }
        };
        fetchFilters();
    }, []);

    // При изменении типа фильтра обновляем доступные направления
    useEffect(() => {
        if (!selectedType) {
            setFilteredDirections([]);
            return;
        }
        const directions = filters.filter(f => f.type === selectedType);
        setFilteredDirections(directions);
    }, [selectedType, filters]);

    const handleChange = (e) => {
        setFormData({
            ...formData,
            [e.target.name]: e.target.value,
        });
    };

    const handleTypeChange = (e) => {
        setSelectedType(e.target.value);
        setSelectedDirections([]); // Очищаем при смене типа
    };

    const handleDirectionChange = (e) => {
        const value = e.target.value;
        setSelectedDirections(value);
    };

    const handleSubmit = async () => {
        try {
            // const token = localStorage.getItem('jwt');

            // const createResponse = await axios.post(
            //     'https://localhost:7106/api/v1/Worker/CreateResume',
            //     formData,
            //     {
            //         headers: { Authorization: `Bearer ${token}` },
            //     }
            // );
            const createResponse = await api.get(API.worker.createResume, formData)
            const resumeId = createResponse.data.id;

            // Добавляем фильтры
            if (selectedDirections.length > 0) {
                // await axios.post(
                //     'https://localhost:7106/api/v1/Worker/AddResumeFilter',
                //     {
                //         id: resumeId,
                //         typeOfActivity_id: selectedDirections,
                //     },
                //     {
                //         headers: { Authorization: `Bearer ${token}` },
                //     }
                // );
                await api.post(API.worker.createResume,{
                    id: resumeId,
                    typeOfActivity_id: selectedDirections,
                });
            }

            setSnackbar({
                open: true,
                message: 'Резюме успешно создано!',
                severity: 'success',
            });

            setTimeout(() => navigate('/MyResume'), 1500);
        } catch {
            setSnackbar({
                open: true,
                message: 'Ошибка при создании резюме',
                severity: 'error',
            });
        }
    };

    const handleCloseSnackbar = () => {
        setSnackbar((prev) => ({ ...prev, open: false }));
    };

    if (loadingEducation) {
        return (
            <Box sx={{ p: 3, textAlign: 'center' }}>
                <CircularProgress />
                <Typography variant="h6" sx={{ mt: 2 }}>Загрузка данных...</Typography>
            </Box>
        );
    }

    return (
        <Container maxWidth="lg" sx={{ mt: 10, mb: 6 }}>
            <Paper elevation={3} sx={{ p: { xs: 3, sm: 4 }, borderRadius: 4, bgcolor: '#fff' }}>
                <Typography variant="h5" fontWeight="bold" align="center" gutterBottom>
                    📝 Выложить резюме
                </Typography>
                <Divider sx={{ mb: 3 }} />
                <Stack spacing={3}>
                    {/* Поле должности */}
                    <TextField
                        label="Должность"
                        name="post"
                        value={formData.post}
                        onChange={handleChange}
                        fullWidth
                        required
                    />
                    {/* Поле города */}
                    <TextField
                        label="Город, в котором желаете устроиться на работу"
                        name="city"
                        value={formData.city}
                        onChange={handleChange}
                        fullWidth
                        required
                    />
                    {/* :Желаемая зарплата */}
                    <TextField
                        label="Желаемая зарплата*"
                        name="wantedSalary"
                        type="number"
                        value={formData.wantedSalary}
                        onChange={handleChange}
                        fullWidth
                    />
                    {/* Образование */}
                    <FormControl fullWidth>
                        <InputLabel id="education-label">Требуемое образование*</InputLabel>
                        <Select
                            labelId="education-label"
                            label="Ваше образование"
                            name="education_id"
                            value={formData.education_id}
                            onChange={handleChange}
                            disabled={loadingEducation}
                        >
                            {loadingEducation ? (
                                <MenuItem disabled>
                                    <CircularProgress size={20} sx={{ mr: 1 }} />
                                    Загрузка...
                                </MenuItem>
                            ) : (
                                educationList.map((edu) => (
                                    <MenuItem key={edu.id} value={edu.id}>
                                        {edu.name}
                                    </MenuItem>
                                ))
                            )}
                        </Select>
                    </FormControl>
                    {/* Выбор типа фильтра */}
                    <FormControl fullWidth>
                        <InputLabel id="filter-type-label">Выберите направление вакансии</InputLabel>
                        <Select
                            labelId="filter-type-label"
                            label="Выберите тип фильтра"
                            value={selectedType}
                            onChange={handleTypeChange}
                        >
                            {[...new Set(filters.map(f => f.type))].map(type => (
                                <MenuItem key={type} value={type}>
                                    {type}
                                </MenuItem>
                            ))}
                        </Select>
                    </FormControl>
                    {/* Мультиселект направлений */}
                    {filteredDirections.length > 0 && (
                        <FormControl fullWidth>
                            <InputLabel id="filter-direction-label">Направления</InputLabel>
                            <Select
                                multiple
                                labelId="filter-direction-label"
                                label="Направления"
                                value={selectedDirections}
                                onChange={handleDirectionChange}
                                renderValue={(selected) => (
                                    <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                                        {selected.map(id => {
                                            const direction = filteredDirections.find(f => f.id === id);
                                            return direction ? (
                                                <Chip key={id} label={direction.direction} color="primary" size="small" sx={{ bgcolor: '#e3f2fd', color: '#1976d2' }} />
                                            ) : null;
                                        })}
                                    </Box>
                                )}
                            >
                                {filteredDirections.map((dir) => (
                                    <MenuItem key={dir.id} value={dir.id}>
                                        {dir.direction}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    )}
                    {/* Опыт работы */}
                    <TextField
                        label="Опыт работы"
                        name="experience"
                        value={formData.experience}
                        onChange={handleChange}
                        fullWidth
                    />
                    {/* Описание */}
                    <TextField
                        label="Описание навыков"
                        name="skill"
                        value={formData.skill}
                        onChange={handleChange}
                        multiline
                        rows={8}
                        fullWidth
                        inputProps={{ maxLength: 1000 }}
                        helperText={`${formData.skill.length}/1000`}
                    />
                    {/* Кнопка публикации */}
                    <Button
                        variant="contained"
                        color="primary"
                        onClick={handleSubmit}
                        fullWidth
                        sx={{ py: 1.5 }}
                    >
                        Опубликовать резюме
                    </Button>
                </Stack>
            </Paper>
            {/* Snackbar */}
            <Snackbar open={snackbar.open} autoHideDuration={3000} onClose={handleCloseSnackbar}>
                <Alert onClose={handleCloseSnackbar} severity={snackbar.severity}>
                    {snackbar.message}
                </Alert>
            </Snackbar>
        </Container>
    );
}