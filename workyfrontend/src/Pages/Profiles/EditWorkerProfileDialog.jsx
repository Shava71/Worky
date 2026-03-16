import React, { useEffect, useState } from "react";
import {
    Dialog, DialogTitle, DialogContent, DialogActions,
    Button, TextField, Stack
} from "@mui/material";

import api from "../../api/myaxios.js";
import { API } from "../../api/routes.js";

export default function EditWorkerProfileDialog({ open, onClose, profile, onUpdated }) {
    const [form, setForm] = useState({
        firstName: "",
        secondName: "",
        surname: "",
        birthday: "",
        email: "",
        phoneNumber: ""
    });

    const [file, setFile] = useState(null);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        if (profile) {
            const worker = profile.worker;
            setForm({
                firstName: worker.first_name || "",
                secondName: worker.second_name || "",
                surname: worker.surname || "",
                birthday: worker.birthday ? worker.birthday : "",
            });
        }
    }, [profile]);

    const handleChange = (e) => {
        setForm(prev => ({
            ...prev,
            [e.target.name]: e.target.value
        }));
    };

    const handleFile = (e) => {
        setFile(e.target.files?.[0] || null);
    };

    const handleSubmit = async () => {
        try {
            setSaving(true);

            // 1. обновляем профиль соискателя
            await api.put(API.worker.updateProfile, {
                firstName: form.firstName,
                secondName: form.secondName,
                surname: form.surname,
                birthday: form.birthday,
                email: form.email,
                phoneNumber: form.phoneNumber
            });

            // 2. если есть фото — загружаем в MinIO через presigned URL
            if (file) {
                const presign = await api.post(API.auth.uploadPhoto(profile.worker.id));
                const presignedUrl = presign.data.url;
                const url = presignedUrl.replace(/^http:\/\/minio:9000/, 'http://localhost:8080/minio');

                const webpFile = await convertToWebp(file);
                await fetch(url, { method: "PUT", body: webpFile });
            }

            onUpdated?.();
            onClose();
        } catch (e) {
            console.error("Ошибка обновления профиля:", e);
        } finally {
            setSaving(false);
        }
    };

    const convertToWebp = (file) => {
        return new Promise((resolve) => {
            const img = new Image();
            const reader = new FileReader();

            reader.onload = (e) => img.src = e.target.result;
            img.onload = () => {
                const canvas = document.createElement("canvas");
                canvas.width = img.width;
                canvas.height = img.height;
                const ctx = canvas.getContext("2d");
                ctx.drawImage(img, 0, 0);
                canvas.toBlob(blob => resolve(blob), "image/webp", 0.9);
            };

            reader.readAsDataURL(file);
        });
    };

    return (
        <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
            <DialogTitle>Редактировать профиль соискателя</DialogTitle>

            <DialogContent>
                <Stack spacing={2} mt={1}>
                    <TextField label="Имя" name="firstName" value={form.firstName} onChange={handleChange} fullWidth />
                    <TextField label="Фамилия" name="secondName" value={form.secondName} onChange={handleChange} fullWidth />
                    <TextField label="Отчество" name="surname" value={form.surname} onChange={handleChange} fullWidth />
                    <TextField label="Дата рождения" name="birthday" type="date" value={form.birthday} onChange={handleChange} fullWidth InputLabelProps={{ shrink: true }} />

                    <Button variant="outlined" component="label">
                        Загрузить фото
                        <input hidden type="file" accept="image/*" onChange={handleFile} />
                    </Button>
                </Stack>
            </DialogContent>

            <DialogActions>
                <Button onClick={onClose}>Отмена</Button>
                <Button variant="contained" onClick={handleSubmit} disabled={saving}>
                    Сохранить
                </Button>
            </DialogActions>
        </Dialog>
    );
}