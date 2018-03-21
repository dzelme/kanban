import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value, Credentials } from './Interfaces';

interface BoardReaderState {
    boardlist: Value[];
    loading: boolean;
    credentials: Credentials;
}

//Get all boards in list
export class BoardReaderFromUrl extends React.Component<RouteComponentProps<{ id: number }>, BoardReaderState> {

    constructor(props: RouteComponentProps<{ id: number }>) {
        super(props);
        this.state = {
            boardlist: [],
            loading: true,
            credentials: { username: "service.kosmoss.tv", password: "ZycsakMylp8od6" }
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
            fetch('api/SampleData/BoardList/?credentials=' + this.state.credentials.username + ":" + this.state.credentials.password, {
                headers: {
                    authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
                }
            })
                .then(handleErrors)
                .then(response => response.json() as Promise<Value[]>)
                .then(data => {
                    this.setState({ boardlist: data }, this.checkVisibility);
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
                }],
                loading: false,
                credentials: { username: "service.kosmoss.tv", password: "ZycsakMylp8od6" } //jānomaina, ka paņem credentials no presentation
            }
        }
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
            : (this.state.boardlist.length != 0) ? <ColumnReader boardlist={this.state.boardlist} credentials={this.state.credentials} /> : <h1>No boards selected</h1>
        
        return<div>{boardInfo}</div>
    }
} 
























