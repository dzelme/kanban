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
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleSubmit(event) {
        event.preventDefault();

        this.state.credentials.username = document.forms['login'].elements["username"].value;
        this.state.credentials.password = document.forms['login'].elements["password"].value;

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
            });
    }

    public render() {
        let contents = Login.renderLogin(this.state.credentials, this.handleSubmit);

        return <div>
            <h1>Login</h1>
            {contents}
        </div>;
    }

    private static renderLogin(credentials: Credentials, handleSubmit) {
        return <form name="login" onSubmit={handleSubmit}>
            <label htmlFor="username">Username</label>
            <input id="username" name="username" type="text" defaultValue={credentials.username} />
            <label htmlFor="password">Password</label>
            <input id="password" name="password" type="password" defaultValue={credentials.password} />
            <input type="submit" className="btn btn-default" name="Submit" />
        </form>;
    }
}