import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { BoardPresentation, Credentials, FullBoardList, Value } from './Interfaces';

interface BoardListState {
    boardPresentation: BoardPresentation;
    boardList: Value[];
    loading: boolean;
}

export class BoardList extends React.Component<RouteComponentProps<{}>, BoardListState> {
    constructor() {
        super();

        this.state = {
            boardPresentation: {
                id: "",
                title: "",
                owner: "",
                credentials: {
                    username: "",
                    password: "",
                },
                boards: {
                    values: []
                }
            },
            boardList: [],
            loading: true
        };

        fetch('api/SampleData/BoardList', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardPresentation: null, boardList: data, loading: false });
            });

        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleSubmit(event) {
        event.preventDefault();
        const data = new FormData(event.target);

        var val = new Array();

        this.state.boardList.map(board => {
            board.visibility = document.forms['boardlist'].elements[board.id + "visibility"].checked;
            if (board.visibility == true) { val.push(board); }
            board.timeShown = parseInt(document.forms['boardlist'].elements[board.id + "timeShown"].value);
            board.refreshRate = parseInt(document.forms['boardlist'].elements[board.id + "refreshRate"].value);
        })

        this.setState({
            boardPresentation: {
                id: document.forms['boardlist'].elements["id"].value,
                title: document.forms['boardlist'].elements["title"].value,
                owner: document.forms['boardlist'].elements["owner"].value,
                credentials: {
                    username: "",
                    password: "",
                },
                boards: {
                    values: val,
                }
            }
        })

        fetch('api/admin/Presentations/', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + sessionStorage.getItem('JwtToken')
            },
            body: JSON.stringify(this.state.boardPresentation),
        });
    }

    public render() {

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : BoardList.renderBoardList(this.state.boardList, this.handleSubmit);

        return <div>

            {contents}
        </div>;
    }

    private static renderBoardList(boardList: Value[], handleSubmit) {  //
        return <div>
            <h1>Izveidot prezentāciju</h1>

            <form name="boardlist" onSubmit={handleSubmit}>
                <div key="id" style={styleForm}>
                    ID: <input id="id" name="id" required type="text" />
                </div>
                <div key="title" style={styleForm}>
                    Nosaukums: <input id="title" required name="title" type="text" />
                </div>
                <div key="owner" style={styleForm}>
                    Izveidotājs: <input id="owner" required name="owner" type="text" />    
                </div>
                <div style={styleButton}><button type="submit" className="btn btn-default">Apstiprināt</button></div>


            <table className='table'>
                <thead style={styleHeader}>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Tips</th>
                        <th>Iekļaut prezentācijā</th>
                        <th>Attēlošanas laiks</th>
                        <th>Atjaunošanas laiks</th>
                    </tr>
                </thead>
                <tbody style={styleContent}>
                    {boardList.map(board =>
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
            
        </form>
        </div>
    }
}
const styleHeader = {
    fontSize: '20px'
}

const styleContent = {
    fontSize: '15px'
}

const styleForm = {
    fontSize: '20px',
    display: 'inline-block',
    margin: '10px',
    marginBottom: '30px'
}

const styleButton = {
    display: 'inline-block',
    height:'40px'
}