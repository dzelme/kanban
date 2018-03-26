import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value, Credentials } from './Interfaces';
import { ApiClient } from './ApiClient';

interface BoardReaderState {
    boardlist: Value[];
    loading: boolean;
    credentials: Credentials;
}

//Get all boards in list
export class AllBoardReader extends React.Component<RouteComponentProps<{}>, BoardReaderState> {

    constructor(props) {
        super(props);
        this.state = {
            boardlist: [],
            loading: true,
            credentials: { username: "service.kosmoss.tv", password:"ZycsakMylp8od6" }
        };

        ApiClient.boardList(this.state.credentials)
            .then(data => {
                this.setState({ boardlist: data, loading: false });
            })

        //fetch('api/SampleData/BoardList/?credentials=' + this.state.credentials.username + ":" + this.state.credentials.password, {
        //    headers: {
        //        authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
        //    }
        //})
        //    .then(response => response.json() as Promise<Value[]>)
        //    .then(data => {
        //        this.setState({ boardlist: data, loading: false });
        //    })
    }

    public render() {

        let boardInfo = this.state.loading
            ? <p><em>Loading all boards</em></p>
            : (this.state.boardlist.length != 0) ? <ColumnReader boardlist={this.state.boardlist} credentials={this.state.credentials} /> : <h1 >No boards</h1>

        return <div>{boardInfo}</div>
    }
}