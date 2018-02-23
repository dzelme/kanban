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

        this.handleSubmit = this.handleSubmit.bind(this);

        //this.handleInputChange = this.handleInputChange.bind(this);  //
    }

    /*
    handleInputChange(event) {
        const target = event.target;
        const value = target.type === 'checkbox' ? target.checked : target.value;
        const name = target.name;

        this.setState({
            [name]: value
        });
    }
    */

    handleSubmit(event) {
        event.preventDefault();
        const data = new FormData(event.target);

        this.state.boardlist.map(board => {
            board.visibility = (data.get(board.id + "visibility") == "on") ? true : false;
            board.timeShown = parseInt(data.get(board.id + "timeShown").toString());
            board.refreshRate = parseInt(data.get(board.id + "refreshRate").toString());
        })

        fetch('api/Form', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(this.state.boardlist),
        });
    }

    public render() {
        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : BoardList.renderBoardList(this.state.boardlist, this.handleSubmit);

        return <div>
            <h1>Board List</h1>
            {contents}
        </div>;
    }

    private static renderBoardList(boardlist: Value[], handleSubmit) {  //
        return <form onSubmit={handleSubmit}>
            <table className='table'>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Type</th>
                        <th>Visibility</th>
                        <th>Time shown</th>
                        <th>Refresh rate</th>
                    </tr>
                </thead>
                <tbody>
                    {boardlist.map(board =>
                        <tr key={board.id + "row"}>
                            <td key={board.id + ""}>{board.id}</td>
                            <td key={board.id + "name"}>{board.name}</td>
                            <td key={board.id + "type"}>{board.type}</td>
                            <td key={board.id + "visibility"}><input name={board.id + "visibility"} type="checkbox" defaultChecked={board.visibility} /></td>
                            <td key={board.id + "timeShown"}><input name={board.id + "timeShown"} type="number" defaultValue={board.timeShown.toString()} /></td>
                            <td key={board.id + "refreshRate"}><input name={board.id + "refreshRate"} type="number" defaultValue={board.refreshRate.toString()} /></td>
                        </tr>
                    )}
                </tbody>
            </table>
            <p><button>Submit</button></p>
        </form>;
    }


}

interface Value {
    id: number;
    name: string;
    type: string;

    visibility: boolean;
    refreshRate: number;
    timeShown: number;
}