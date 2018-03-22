import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export class Logout extends React.Component<RouteComponentProps<{}> > {

    constructor(props) {
        super(props);
        this.state = {}

        sessionStorage.removeItem("JwtToken");
        open('./login', '_self');
    }

    render() {
        return null;
    }

}