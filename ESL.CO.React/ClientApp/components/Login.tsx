import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { AuthenticationState } from './Interfaces';
import { ApiClient } from './ApiClient';

export class Login extends React.Component<RouteComponentProps<{}>, AuthenticationState> {

    constructor(props) {
        super(props);
        this.state = {
            credentials: { username: "", password: "" },
            authenticated: true,
        }

        this.isAuthenticated();

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    isAuthenticated() {
        ApiClient.hasValidJwt(false)
            .then(valid => valid && window.open('./admin', '_self'));
    }

    handleChange(event) {
        const name = event.target.name;
        this.setState({ credentials: { ...this.state.credentials, [event.target.name]: event.target.value } });
    }

    handleSubmit(event) {
        event.preventDefault();

        ApiClient.login(this.state.credentials)
            .then(response => {
                if (response) {
                    this.setState({ authenticated: true });
                    open('./admin/presentations', '_self');
                }
                else {
                    this.setState({ authenticated: false });
                }
            })
    }

    public render() {

        let error = this.state.authenticated
            ? null
            : <h4>Nekorekts lietotājvārds un/vai parole!</h4>

        return <div>
            <form onSubmit={this.handleSubmit}>
                <div className="LoginCenter">
                            <h1>Autentifikācija</h1>
                        </div>

                <div className="LoginCenter">
                    <h3>Lietotājvārds</h3>
                    <input className="LoginInputElement" name='username' type="text" autoFocus required value={this.state.credentials.username} onChange={this.handleChange} />
                        </div>

                <div className="LoginCenter">
                    <h3>Parole</h3>
                    <input className="LoginInputElement" name='password' type="password" required value={this.state.credentials.password} onChange={this.handleChange} />
                        </div>

                <div className="LoginCenter">
                            <button className="LoginButton" type="submit"><strong>Apstiprināt</strong></button>
                        </div>
                </form>

            <div className="LoginCenter">{error}</div>

        </div>
    }
}