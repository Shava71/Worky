import React, { useState } from "react";
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    Button,
    TextField,
    Stack
} from "@mui/material";
import api from "../api/axios";
import { API } from "../api/routes";

export default function EditCompanyProfileDialog({
                                                     open,
                                                     onClose,
                                                     company,
                                                     onUpdated
                                                 }) {

    const [form, setForm] = useState(company || {});
    const [photo, setPhoto] = useState(null);

    const handleChange = (e) => {
        setForm({
            ...form,
            [e.target.name]: e.target.value
        });
    };

    const handlePhotoChange = (e) => {
        setPhoto(e.target.files[0]);
    };

    const handleSubmit = async () => {
        try {

            await api.put(API.company.update, form);

            if (photo) {
                const fd = new FormData();
                fd.append("file", photo);

                await api.post(API.company.uploadPhoto, fd);
            }

            onUpdated();
            onClose();

        } catch (e) {
            console.error("Ошибка обновления профиля", e);
        }
    };

    return (
        <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
            <DialogTitle>Редактирование профиля</DialogTitle>

            <DialogContent>

                <Stack spacing={2} mt={1}>

                    <TextField
                        label="Название"
                        name="name"
                        value={form.name || ""}
                        onChange={handleChange}
                        fullWidth
                    />

                    <TextField
                        label="Email"
                        name="email"
                        value={form.email || ""}
                        onChange={handleChange}
                        fullWidth
                    />

                    <TextField
                        label="Телефон"
                        name="phoneNumber"
                        value={form.phoneNumber || ""}
                        onChange={handleChange}
                        fullWidth
                    />

                    <TextField
                        label="Сайт"
                        name="website"
                        value={form.website || ""}
                        onChange={handleChange}
                        fullWidth
                    />

                    <TextField
                        label="Latitude"
                        name="latitude"
                        value={form.latitude || ""}
                        onChange={handleChange}
                    />

                    <TextField
                        label="Longitude"
                        name="longitude"
                        value={form.longitude || ""}
                        onChange={handleChange}
                    />

                    <Button
                        variant="outlined"
                        component="label"
                    >
                        Загрузить фото
                        <input
                            hidden
                            type="file"
                            accept="image/*"
                            onChange={handlePhotoChange}
                        />
                    </Button>

                </Stack>

            </DialogContent>

            <DialogActions>

                <Button onClick={onClose}>
                    Отмена
                </Button>

                <Button
                    variant="contained"
                    onClick={handleSubmit}
                >
                    Сохранить
                </Button>

            </DialogActions>

        </Dialog>
    );
}