import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value, Credentials, BoardPresentation } from './Interfaces';

interface BoardReaderState {
    boardlist: Value[];
    credentials: Credentials;
    loading: boolean;
}

//Get all boards in list
export class BoardReader extends React.Component<RouteComponentProps<{ id: number }>, BoardReaderState> {

    constructor(props: RouteComponentProps<{ id: number }>) {
        super(props);
        this.state = {
            boardlist: [],
            credentials: { username: "", password: "" },
            loading: true
        };

        //client offline error
        function handleErrors(response) {
            if (response.status == 401) {
                open('/login', '_self');
                return response;
            }
            if (!response.ok) {
                throw Error(response.statusText);
            }
            return response;
        }

        fetch('api/admin/Presentations/' + this.props.match.params.id, {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(handleErrors)
            .then(response => response.json() as Promise<BoardPresentation>)
            .then(data => {
                this.setState({ boardlist: data.boards.values, credentials: { username: data.credentials.username, password: data.credentials.password }, loading: false });
            });
    }

    public render() {
        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.boardlist.length != 0) ? <ColumnReader boardlist={this.state.boardlist} credentials={this.state.credentials} /> : <h1 >No boards selected</h1>
        
        return<div>{boardInfo}</div>
    }
} 
























