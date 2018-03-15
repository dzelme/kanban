import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface Auth {
    login: string;
    password: string;
}


export class Authentication_UI extends React.Component<RouteComponentProps<{}>, Auth> {

    constructor(props) {
        super(props);
        this.state = { login: '', password:'' }

        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(event)
    {
        const target = event.target;
        const name = target.name;

        this.setState({
            [name]: target.value
        });
    }

    handleSubmit(event)
    {
        //alert('Login: ' + this.state.login + ' Password: ' + this.state.password);
        
        event.preventDefault();
    }

    public render() {

        return <form onSubmit={this.handleSubmit}>
            <div style={styleCenter}>
                <h1>Autentifikācija</h1>
            </div>

            <div style={styleCenter}>
                <h3>Lietotājvārds</h3>
                <input style={LoginInputStyle} name='login' type="text" required value={this.state.login} onChange={this.handleChange} />
            </div>

            <div style={styleCenter}>
                <h3>Parole</h3>
                <input style={LoginInputStyle} name='password' type="password" required value={this.state.password} onChange={this.handleChange} />
            </div>

            <div style={styleCenter}>
                <button style={buttonStyle} type="submit" disabled={!this.state.login || !this.state.password}><strong>Apstiprināt</strong></button>
            </div>
        </form>
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