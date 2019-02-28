import React from 'react';
import { AppContext } from '../../AppContext';
import Navbar from '../../components/navbar/Navbar';

export default class EditVideoPage extends React.Component {
    render() {
        return <div className="editVideoPage">
            <Navbar />
        </div>
    }
}

EditVideoPage.contextType = AppContext;