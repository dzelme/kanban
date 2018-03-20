import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import ColumnReader from './ColumnReader';
import { Value } from './Interfaces';

interface BoardReaderState {
    boardlist: Value[];
    loading: boolean;
}

//Get all boards in list
export class AllBoardReader extends React.Component<RouteComponentProps<{}>, BoardReaderState> {

    constructor(props) {
        super(props);
        this.state = {
            boardlist: [],
            loading: true
        };

        fetch('api/SampleData/BoardList/?credentials=service.kosmoss.tv:ZycsakMylp8od6', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardlist: data, loading: false });
            })
    }

    public render() {

        let boardInfo = this.state.loading
            ? <p><em>Loading...</em></p>
            : (this.state.boardlist.length != 0) ? <ColumnReader boardlist={this.state.boardlist} /> : <h1 >No boards</h1>

        return <div>{boardInfo}</div>
    }
}