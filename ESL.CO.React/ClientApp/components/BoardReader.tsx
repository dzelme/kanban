import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value } from './Interfaces';

const styleBoard = {
    maxWidth: '2000'
}

//const styleColumnNext = {
//    borderBottom: 'solid',
//    borderTop: 'solid',
//    borderRight: 'solid',
//};

interface BoardReaderState {
    boardlist: Value[];
    loading: boolean;
}

//Get all boards in list
export class BoardReader extends React.Component<RouteComponentProps<{}>, BoardReaderState> {

    constructor() {
        super();
        this.state = {
            boardlist: [],
            loading: true
        };

        fetch('api/SampleData/BoardList')
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardlist: data }, this.checkVisibility);
            });
    }

    checkVisibility() {
        var allBoards = this.state.boardlist;
        const visibleBoardList = [];

        allBoards.map((board) => {
            if (board.visibility) {
                visibleBoardList.push(board);
            }

        })

        this.setState({ boardlist: visibleBoardList, loading: false });
    }

    public render() {
        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : <ColumnReader boardlist={this.state.boardlist} />

        return <div style={styleBoard}>{boardInfo}</div>
    }
} 



























