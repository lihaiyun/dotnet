import React, { useState } from 'react';
import { Box, Typography, TextField, Button, Grid } from '@mui/material';
import { FormControl, InputLabel, FormHelperText, Select, MenuItem } from '@mui/material';
import { Checkbox, FormControlLabel } from '@mui/material';
// npm install @mui/x-date-pickers
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { DatePicker } from '@mui/x-date-pickers/DatePicker';
import { TimePicker } from '@mui/x-date-pickers/TimePicker';
import dayjs from 'dayjs';
import { useFormik } from 'formik';
import * as yup from 'yup';
import http from '../http';
import { ToastContainer, toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';

function MyForm() {
    const [imageFile, setImageFile] = useState(null);

    const formik = useFormik({
        initialValues: {
            title: 'My title',
            description: 'My description',
            age: 10,
            price: 0,
            option: '',
            tnc: false,
            date: dayjs(),
            time: dayjs().minute(0)
        },
        validationSchema: yup.object({
            title: yup.string().trim()
                .min(3, 'Title must be at least 3 characters')
                .max(100, 'Title must be at most 100 characters')
                .required('Title is required'),
            description: yup.string().trim()
                .min(3, 'Description must be at least 3 characters')
                .max(500, 'Description must be at most 500 characters')
                .required('Description is required'),
            price: yup.number().min(0).required('Price is required'),
            option: yup.string().required('Option is required'),
            tnc: yup.boolean().oneOf([true], 'Accept Terms & Conditions is required')
        }),
        onSubmit: (data) => {
            if (imageFile) {
                data.imageFile = imageFile;
            }
            data.title = data.title.trim();
            data.description = data.description.trim();
            data.date = data.date.format('YYYY-MM-DD');
            data.time = data.time.format('HH:mm');
            console.log(data);
            toast.success('Form submitted successfully');
        }
    });

    const onFileChange = (e) => {
        let file = e.target.files[0];
        if (file) {
            if (file.size > 1024 * 1024) {
                toast.error('Maximum file size is 1MB');
                return;
            }

            let formData = new FormData();
            formData.append('file', file);
            http.post('/file/upload', formData, {
                headers: {
                    'Content-Type': 'multipart/form-data'
                }
            })
                .then((res) => {
                    setImageFile(res.data.filename);
                })
                .catch(function (error) {
                    console.log(error.response);
                });
        }
    };

    return (
        <Box>
            <Typography variant="h5" sx={{ my: 2 }}>
                My Example Form
            </Typography>
            <Box component="form" onSubmit={formik.handleSubmit}>
                <Grid container spacing={2}>
                    <Grid item xs={12} md={6} lg={8}>
                        <TextField
                            fullWidth margin="dense" autoComplete="off"
                            label="Title"
                            name="title"
                            value={formik.values.title}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            error={formik.touched.title && Boolean(formik.errors.title)}
                            helperText={formik.touched.title && formik.errors.title}
                        />
                        <TextField
                            fullWidth margin="dense" autoComplete="off"
                            multiline minRows={2}
                            label="Description"
                            name="description"
                            value={formik.values.description}
                            onChange={formik.handleChange}
                            onBlur={formik.handleBlur}
                            error={formik.touched.description && Boolean(formik.errors.description)}
                            helperText={formik.touched.description && formik.errors.description}
                        />

                        <Grid container spacing={2}>
                            <Grid item xs={12} md={6}>
                                <TextField
                                    fullWidth margin="dense" autoComplete="off"
                                    type="number"
                                    inputProps={{
                                        min: 0,
                                        step: 0.1,
                                    }}
                                    label="Price"
                                    name="price"
                                    value={formik.values.price}
                                    onChange={formik.handleChange}
                                    onBlur={formik.handleBlur}
                                    error={formik.touched.price && Boolean(formik.errors.price)}
                                    helperText={formik.touched.price && formik.errors.price} />
                            </Grid>
                            <Grid item xs={12} md={6}>
                                <FormControl fullWidth margin="dense"
                                    error={formik.touched.option && Boolean(formik.errors.option)}>
                                    <InputLabel>Option</InputLabel>
                                    <Select label="Option"
                                        name="option"
                                        value={formik.values.option}
                                        onChange={formik.handleChange}
                                        onBlur={formik.handleBlur}
                                    >
                                        <MenuItem value={'A'}>Option A</MenuItem>
                                        <MenuItem value={'B'}>Option B</MenuItem>
                                        <MenuItem value={'C'}>Option C</MenuItem>
                                        <MenuItem value={'D'}>Option D</MenuItem>
                                    </Select>
                                    <FormHelperText>{formik.touched.option && formik.errors.option}</FormHelperText>
                                </FormControl>
                            </Grid>
                            <Grid item xs={12} md={6}>
                                <FormControl fullWidth>
                                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                                        <DatePicker format="DD/MM/YYYY"
                                            label="Select Date"
                                            name="date"
                                            value={formik.values.date}
                                            onChange={(date) => formik.setFieldValue('date', date)}
                                            onBlur={() => formik.setFieldTouched('date', true)} />
                                    </LocalizationProvider>
                                </FormControl>
                            </Grid>
                            <Grid item xs={12} md={6}>
                                <FormControl fullWidth>
                                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                                        <TimePicker
                                            label="Select Time"
                                            name="time"
                                            value={formik.values.time}
                                            onChange={(time) => formik.setFieldValue('time', time)}
                                            onBlur={() => formik.setFieldTouched('time', true)} />
                                    </LocalizationProvider>
                                </FormControl>
                            </Grid>
                        </Grid>

                        <FormControl fullWidth margin="dense"
                            error={formik.touched.tnc && Boolean(formik.errors.tnc)}>
                            <FormControlLabel control={
                                <Checkbox name="tnc"
                                    checked={formik.values.tnc}
                                    onChange={formik.handleChange}
                                    onBlur={formik.handleBlur} />
                            } label="I Accept Terms & Conditions" />
                            <FormHelperText>{formik.touched.tnc && formik.errors.tnc}</FormHelperText>
                        </FormControl>
                    </Grid>

                    <Grid item xs={12} md={6} lg={4}>
                        <Box sx={{ textAlign: 'center', mt: 2 }} >
                            <Button variant="contained" component="label">
                                Upload Image
                                <input hidden accept="image/*" multiple type="file"
                                    onChange={onFileChange} />
                            </Button>
                            {
                                imageFile && (
                                    <Box className="aspect-ratio-container" sx={{ mt: 2 }}>
                                        <img alt="tutorial"
                                            src={`${import.meta.env.VITE_FILE_BASE_URL}${imageFile}`}>
                                        </img>
                                    </Box>
                                )
                            }
                        </Box>
                    </Grid>
                </Grid>
                <Box sx={{ mt: 2 }}>
                    <Button variant="contained" type="submit">
                        Submit
                    </Button>
                </Box>
            </Box>

            <ToastContainer />
        </Box>
    );
}

export default MyForm;