import * as React from 'react';
import { RouteComponentProps } from 'react-router';

interface Auth {
    login: string;
    password: string;
}


export default class Authentication_UI extends React.Component<{ onClick: any }, Auth> {

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


        const isEnabled = this.state.login.length > 0 && this.state.password.length > 0;

        return <form onSubmit={this.handleSubmit}>
                <h1>Autentifikācija</h1>

                <h3>Lietotājvārds</h3>
                <input style={LoginInputStyle} name='login' type="text" value={this.state.login} onChange={this.handleChange} />

                <h3>Parole</h3>
                <input style={LoginInputStyle} name='password' type="password" value={this.state.password} onChange={this.handleChange} />

                <div> <button style={buttonStyle} type="submit" value="Apstiprināt" disabled={!isEnabled} onClick={this.props.onClick}>Apstiprināt</button></div>
        </form>
    }
}

const buttonStyle = {
    color: 'black',
    width: '150px',
    marginTop: '20px',
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