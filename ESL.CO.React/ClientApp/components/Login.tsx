import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Credentials } from './Interfaces';

interface LoginState {
    credentials: Credentials;
}

export class Login extends React.Component<RouteComponentProps<{}>, LoginState> {
    constructor() {
        super();
        this.state = {
            credentials: { username: "", password: "" },
        };
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event) {
        const name = event.target.name;
        this.setState({ credentials: { ...this.state.credentials, [event.target.name]: event.target.value } });
    }

    handleSubmit(event) {
        event.preventDefault();

        fetch(' /api/account/login', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(this.state.credentials),
        })
            .then(response => response.json())
            .then(response => {
                if (response.token != null) { sessionStorage.setItem('JwtToken', response.token); }
            });
    }

    public render() {
        let contents = Login.renderLogin(this.state.credentials, this.handleSubmit, this.handleChange);

        return <div>
            <h1>Login</h1>
            {contents}
        </div>;
    }

    private static renderLogin(credentials: Credentials, handleSubmit, handleChange) {
        return <form name="login" onSubmit={handleSubmit}>
            <label>
                Username:
                <input id="username" name="username" type="text" value={credentials.username} onChange={handleChange} />
            </label>
            <label>
                Password:
                <input id="password" name="password" type="password" value={credentials.password} onChange={handleChange} />
            </label>
            <input type="submit" className="btn btn-default" name="Submit" />
        </form>;
    }
}