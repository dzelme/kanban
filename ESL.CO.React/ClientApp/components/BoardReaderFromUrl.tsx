import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value } from './Interfaces';

interface BoardReaderState {
    boardList: Value[];
    loading: boolean;
}

//Get all boards in list
export class BoardReaderFromUrl extends React.Component<RouteComponentProps<{ id: number }>, BoardReaderState> {

    constructor(props: RouteComponentProps<{ id: number }>) {
        super(props);
        this.state = {
            boardList: [],
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

        if (this.props.match.params.id == null) {
            fetch('api/SampleData/BoardList', {
                headers: {
                    authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
                }
            })
                .then(handleErrors)
                .then(response => response.json() as Promise<Value[]>)
                .then(data => {
                    this.setState({ boardList: data }, this.checkVisibility);
                })
                .catch(error => console.log(error));
        }
        else {
            this.state = {
                boardList: [{
                    id: this.props.match.params.id,
                    name: "test",
                    type: "test",
                    visibility: true,
                    timeShown: 10000,
                    refreshRate: 5000,
                }],
                loading: false
            }
        }
    }

    checkVisibility() {
        var allBoards = this.state.boardList;
        const visibleBoardList = [];

        allBoards.map((board) => {
            if (board.visibility) {
                visibleBoardList.push(board);
            }

        })

        this.setState({ boardList: visibleBoardList, loading: false });

    }

    public render() {
        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.boardList.length != 0) ? <ColumnReader boardList={this.state.boardList} /> : <h1>No boards selected</h1>
        
        return<div>{boardInfo}</div>
    }
} 
























