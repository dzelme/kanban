import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value } from './Interfaces';

const styleBoard = {
    maxWidth: '2000px'
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

        //client offline error
        function handleErrors(response) {
            if (!response.ok) {
                throw Error(response.statusText);
            }
            return response;
        }

        fetch('api/SampleData/BoardList')
            .then(handleErrors)
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardlist: data }, this.checkVisibility);
            })
            .catch(error => console.log(error));
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



























