import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value, BoardPresentation } from './Interfaces';

interface BoardReaderState {
    boardList: Value[];
    loading: boolean;
}

//Get all boards in list
export class BoardReader extends React.Component<RouteComponentProps<{ id: number }>, BoardReaderState> {

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

        fetch('api/admin/Presentations/' + this.props.match.params.id, {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(handleErrors)
            .then(response => response.json() as Promise<BoardPresentation>)
            .then(data => {
                this.setState({ boardList: data.boards.values, loading: false });
            });
    }

    public render() {
        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.boardList.length != 0) ? <ColumnReader boardList={this.state.boardList} /> : <h1>No boards selected</h1>
        
        return<div>{boardInfo}</div>
    }
} 
























