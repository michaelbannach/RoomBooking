// src/components/Navbar.jsx
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";

export default function Navbar({ currentView, onChangeView }) {
    return (
        <AppBar position="static" color="primary" elevation={1}>
            <Toolbar>
                <Typography
                    variant="h6"
                    component="div"
                    sx={{ flexGrow: 1, fontWeight: 500 }}
                >
                    RoomBooking
                </Typography>

                <Box sx={{ display: "flex", gap: 1 }}>
                    <Button
                        color="inherit"
                        size="small"
                        variant={currentView === "resourceTimeGridDay" ? "outlined" : "text"}
                        onClick={() => onChangeView("resourceTimeGridDay")}
                    >
                        Tagesansicht
                    </Button>

                    <Button
                        color="inherit"
                        size="small"
                        variant={currentView === "resourceTimeGridWeek" ? "outlined" : "text"}
                        onClick={() => onChangeView("resourceTimeGridWeek")}
                    >
                        Wochenansicht
                    </Button>
                </Box>
            </Toolbar>
        </AppBar>
    );
}
