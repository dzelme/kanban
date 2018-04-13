import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { BoardReaderState, StatisticsDbModel } from './Interfaces';
import { ApiClient } from './ApiClient';

export class BoardReader extends React.Component<RouteComponentProps<{ id: string }>, BoardReaderState> {

    constructor(props: RouteComponentProps<{ id: string }>) {
        super(props);
        this.state = {
            boardList: [],
            loading: true,
            titleList:[]
        };

        ApiClient.getPresentation(this.props.match.params.id)
            .then(data => {
                this.setState({ boardList: data.boards.values }, this.makeTitleList);
            });

        let stats = {
            presentationId: this.props.match.params.id,
            boardId: "",
            type: "presentation"
        }
        ApiClient.saveViewStatistics(stats);
    }

    makeTitleList() {
        let list = [];

        this.state.boardList.map(board =>
            list.push(board.name)
        )

        this.setState({ titleList: list, loading: false });
    }

    public render() {
        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.boardList.length != 0) ? <ColumnReader boardList={this.state.boardList} presentationId={this.props.match.params.id} titleList={this.state.titleList} /> : <h1 >No boards selected</h1>
        
        return<div>{boardInfo}</div>
    }
}