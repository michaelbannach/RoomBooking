// src/components/Navbar.jsx
import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";

export default function Navbar() {
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
                    <Button color="inherit" size="small">
                        Tagesansicht
                    </Button>
                    <Button color="inherit" size="small">
                        Wochenansicht
                    </Button>
                    <Button variant="outlined" color="inherit" size="small">
                        Jetzt buchen
                    </Button>
                </Box>
            </Toolbar>
        </AppBar>
    );
}
