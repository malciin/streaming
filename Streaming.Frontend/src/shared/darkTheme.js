import React from 'react';
import { createMuiTheme, MuiThemeProvider } from '@material-ui/core/styles';

class DarkTheme extends React.Component {
    
    render() {
        const theme = createMuiTheme({
            palette: {
                type: 'dark'
            },
            typography: {
                useNextVariants: true,
            },
        });

        return (
            <MuiThemeProvider theme={theme}>
                {this.props.children}
            </MuiThemeProvider>
        );
    }
}

export default DarkTheme;