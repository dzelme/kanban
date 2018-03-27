import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Credentials } from './Interfaces';
import { ApiClient } from './ApiClient';

interface AuthenticationState {
    credentials: Credentials;
    authenticated: boolean;
}

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
        ApiClient.hasValidJwt()
            .then(response => ApiClient.redirect(response, 200, './admin'));
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

const LoginInputStyle = {
    color: 'black',
    width:'150px'
}

const styleCenter = {
    height: '100 %',
    width: '100 %',
    display: 'flex',
    justifyContent: 'center' as 'center',
    alignItems: 'center' as 'center'
}