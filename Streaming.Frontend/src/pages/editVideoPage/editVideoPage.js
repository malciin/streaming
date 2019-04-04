import React from 'react';
import { AppContext } from '../../appContext';
import Navbar from '../../components/navbar/navbar';

export default class EditVideoPage extends React.Component {
    render() {
        return <div className="editVideoPage">
            <Navbar />
        </div>
    }
}

EditVideoPage.contextType = AppContext;