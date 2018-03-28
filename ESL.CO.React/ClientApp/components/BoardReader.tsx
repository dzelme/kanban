import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value, Credentials, BoardPresentation } from './Interfaces';
import { ApiClient } from './ApiClient';

interface BoardReaderState {
    boardlist: Value[];
    loading: boolean;
    titleList: string[];
}

//Get all boards in list
export class BoardReader extends React.Component<RouteComponentProps<{ id: string }>, BoardReaderState> {

    constructor(props: RouteComponentProps<{ id: string }>) {
        super(props);
        this.state = {
            boardlist: [],
            loading: true,
            titleList:[]
        };

        ApiClient.getAPresentation(this.props.match.params.id)
            .then(data => {
                this.setState({ boardlist: data.boards.values }, this.makeTitleList);
            });
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
            ? <p><em>Loading...</em></p>
            : (this.state.boardlist.length != 0) ? <ColumnReader boardlist={this.state.boardlist} presID={this.props.match.params.id} titleList={this.state.titleList} /> : <h1 >No boards selected</h1>
        
        return<div>{boardInfo}</div>
    }
}