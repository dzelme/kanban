import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';

interface FetchDataExampleState {
    boardlist: Value[];
    loading: boolean;
}

export class BoardList extends React.Component<RouteComponentProps<{}>, FetchDataExampleState> {
    constructor() {
        super();
        this.state = { boardlist: [], loading: true };

        fetch('api/SampleData/BoardList')
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardlist: data, loading: false });
            });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : BoardList.renderBoardList(this.state.boardlist);

        return <div>
            <h1>Board List</h1>
            {contents}
        </div>;
    }

    private static renderBoardList(boardlist: Value[]) {
        return <table className='table'>
            <thead>
                <tr>
                    <th>Id</th>
                    <th>Name</th>
                    <th>Type</th>
                </tr>
            </thead>
            <tbody>
                {boardlist.map(board =>
                    <tr key={board.id}>
                        <td>{board.id}</td>
                        <td>{board.name}</td>
                        <td>{board.type}</td>
                    </tr>
                )}
            </tbody>
        </table>;
    }


}

interface Value {
    id: number;
    name: string;
    type: string;
}