import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value, Credentials } from './Interfaces';
import { ApiClient } from './ApiClient';

interface BoardReaderState {
    boardlist: Value[];
    loading: boolean;
    credentials: Credentials;
    titleList: string[];
}

export class BoardReaderFromUrl extends React.Component<RouteComponentProps<{ id: number }>, BoardReaderState> {

    constructor(props: RouteComponentProps<{ id: number }>) {
        super(props);
        this.state = {
            boardlist: [],
            loading: true,
            credentials: { username: "service.kosmoss.tv", password: "ZycsakMylp8od6" },
            titleList:[]
        };

        if (this.props.match.params.id == null) {
            ApiClient.boardList(this.state.credentials)
                .then(data => {
                    this.setState({ boardlist: data, loading: false });
                })
                .catch(error => console.log(error));
        }
        else {
            this.state = {
                boardlist: [{
                    id: this.props.match.params.id,
                    name: "test",
                    type: "test",
                    visibility: true,
                    timeShown: 10000,
                    refreshRate: 5000,
                    timesShown: 0,
                    lastShown: ""
                }],
                loading: false,
                credentials: this.state.credentials,
                titleList:[]
            }
        }
    }

    public render() {
        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.boardlist.length != 0) ? <ColumnReader boardlist={this.state.boardlist} credentials={this.state.credentials} titleList={this.state.titleList} /> : <h1>No boards selected</h1>
        
        return<div>{boardInfo}</div>
    }
} 
























