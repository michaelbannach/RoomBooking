import AppBar from "@mui/material/AppBar";
import Toolbar from "@mui/material/Toolbar";
import Typography from "@mui/material/Typography";
import Button from "@mui/material/Button";
import Box from "@mui/material/Box";
import IconButton from "@mui/material/IconButton";
import LogoutIcon from "@mui/icons-material/Logout";

export default function Navbar({ currentView, onChangeView }) {
    const handleLogout = () => {
        localStorage.removeItem("jwt");
        window.location.href = "/login";
    };

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

                <Box sx={{ display: "flex", alignItems: "center", gap: 1, mr: 1 }}>
                    <Button
                        color="inherit"
                        size="small"
                        variant={
                            currentView === "resourceTimeGridDay" ? "outlined" : "text"
                        }
                        onClick={() => onChangeView("resourceTimeGridDay")}
                    >
                        Tagesansicht
                    </Button>

                    <Button
                        color="inherit"
                        size="small"
                        variant={
                            currentView === "resourceTimeGridWeek" ? "outlined" : "text"
                        }
                        onClick={() => onChangeView("resourceTimeGridWeek")}
                    >
                        Wochenansicht
                    </Button>
                </Box>

                <IconButton
                    color="inherit"
                    size="small"
                    onClick={handleLogout}
                    edge="end"
                    aria-label="Logout"
                >
                    <LogoutIcon />
                </IconButton>
            </Toolbar>
        </AppBar>
    );
}
