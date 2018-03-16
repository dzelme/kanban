﻿import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Credentials } from './Interfaces';

interface Auth {
    credentials: Credentials;
    errorAuth: boolean;
}


export class Authentication_UI extends React.Component<RouteComponentProps<{}>,Auth> {

    constructor(props) {
        super(props);
        this.state = {
            credentials: { username: "", password: "" },
            errorAuth: false,
        },

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event)
    {
        const name = event.target.name;
        this.setState({ credentials: { ...this.state.credentials, [event.target.name]: event.target.value } });
    }

    handleSubmit(event)
    {
        event.preventDefault();

        fetch(' /api/account/login', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(this.state.credentials),
        })
            .then(response => {
                console.log('response:', response.status);
                if (response.status == 401) {
                    this.setState({ errorAuth: true });
                }
                else if (response.status == 200) {
                    this.setState({ errorAuth: false });
                    open('/admin');
                }
            });
    }

    public render() {

        let error = this.state.errorAuth
            ? <h4>Nekorekts lietotājvārds un/vai parole!</h4>
            : null

        return <div>
                <form onSubmit={this.handleSubmit}>
                        <div style={styleCenter}>
                            <h1>Autentifikācija</h1>
                        </div>

                        <div style={styleCenter}>
                    <h3>Lietotājvārds</h3>
                    <input style={LoginInputStyle} name='username' type="text" required value={this.state.credentials.username} onChange={this.handleChange} />
                        </div>

                        <div style={styleCenter}>
                    <h3>Parole</h3>
                    <input style={LoginInputStyle} name='password' type="password" required value={this.state.credentials.password} onChange={this.handleChange} />
                        </div>

                        <div style={styleCenter}>
                            <button style={buttonStyle} type="submit"><strong>Apstiprināt</strong></button>
                        </div>
                </form>

        <div style={styleCenter}>{error}</div>

        </div>
    }
}

const buttonStyle = {
    color: 'black',
    width: '150px',
    marginTop: '20px',
    height:'30px'
}

const Style = {
    color: 'black'
}

const LoginInputStyle = {
    color: 'black',
    width:'150px'
}

const PasswordInputStyle = {
    width: '150px'
}

const styleCenter = {
    height: '100 %',
    width: '100 %',
    display: 'flex',
    justifyContent: 'center' as 'center',
    alignItems: 'center' as 'center'
}

const styleCenterTitle = {
    height: '100 %',
    width: '100 %',
    display: 'flex',
    justifyContent: 'center' as 'center',
    alignItems: 'center' as 'center',

}