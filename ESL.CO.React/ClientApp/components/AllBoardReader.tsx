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

//Get all boards in list
export class AllBoardReader extends React.Component<RouteComponentProps<{}>, BoardReaderState> {

    constructor(props) {
        super(props);
        this.state = {
            boardlist: [],
            loading: true,
            credentials: { username: "service.kosmoss.tv", password: "ZycsakMylp8od6" },
            titleList: []
        };

        ApiClient.boardList(this.state.credentials)
            .then(data => {
                this.setState({ boardlist: data }, this.makeTitleList);
            })
    }


    makeTitleList() {
        let list = [];

        this.state.boardlist.map(board =>
            list.push(board.name)
        )

        this.setState({ titleList: list, loading: false });
    }

    public render() {

        let boardInfo = this.state.loading
            ? <p><em>Loading all boards</em></p>
            : (this.state.boardlist.length != 0) ? <ColumnReader boardlist={this.state.boardlist} credentials={this.state.credentials} titleList={this.state.titleList} /> : <h1 >No boards</h1>

        return <div>{boardInfo}</div>
    }
}