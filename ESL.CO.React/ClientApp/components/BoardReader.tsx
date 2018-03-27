import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value, Credentials, BoardPresentation } from './Interfaces';
import { ApiClient } from './ApiClient';

interface BoardReaderState {
    boardlist: Value[];
    credentials: Credentials;
    loading: boolean;
}

//Get all boards in list
export class BoardReader extends React.Component<RouteComponentProps<{ id: string }>, BoardReaderState> {

    constructor(props: RouteComponentProps<{ id: string }>) {
        super(props);
        this.state = {
            boardlist: [],
            credentials: { username: "", password: "" },
            loading: true
        };

        ApiClient.getAPresentation(this.props.match.params.id)
            .then(data => {
                this.setState({
                    boardlist: data.boards.values,
                    credentials: { username: data.credentials.username, password: data.credentials.password },
                    loading: false
                });
            });
    }

    public render() {
        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.boardlist.length != 0) ? <ColumnReader boardlist={this.state.boardlist} credentials={this.state.credentials} /> : <h1 >No boards selected</h1>
        
        return<div>{boardInfo}</div>
    }
}