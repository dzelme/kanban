import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Credentials, FullBoardList, Value } from './Interfaces';
import jwt_decode from 'jwt-decode';

interface BoardListState {
    boardPresentation: BoardPresentation;
    boardList: Value[];
    loading: boolean;
    credentials: Credentials;
    invalidCredentials: boolean;
}

export class BoardList extends React.Component<RouteComponentProps<{}>, BoardListState> {
    constructor() {
        super();

        this.state = {
            boardPresentation: {
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
            loading: true,
            credentials: { username: "", password: "" },
            invalidCredentials: false
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleForm = this.handleForm.bind(this);
        this.handleAuth = this.handleAuth.bind(this);
        this.handleFetch = this.handleFetch.bind(this);
        this.postPresentation = this.postPresentation.bind(this);
    }

    handleAuth() {
        event.preventDefault();

        fetch('./api/account/login', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(this.state.credentials),
        })
            .then(response => {
                if (response.ok) {
                    this.setState({ invalidCredentials: false }, this.handleFetch);
                }
                else {
                    this.setState({ invalidCredentials: true });
                }
            });
    }

    handleForm(event) {
        event.preventDefault();

        var username = document.forms['presentation'].elements["username"].value;
        var password = document.forms['presentation'].elements["password"].value;

        this.setState({ credentials: { username: username, password: password }, loading: true }, this.handleAuth)

    }

    handleFetch() {

        function handleErrors(response) {
            if (response.status == 401) {
                open('./login', '_self');
            }
            else if (!response.ok) {
                throw Error(response.statusText);
            }
            return response;
        }

        fetch('api/SampleData/BoardList/?credentials=' + this.state.credentials.username + ":" + this.state.credentials.password, {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(handleErrors)
            .then(response => response.json() as Promise<Value[]>)
            .then(data => {
                this.setState({ boardPresentation: null, boardList: data, loading: false });
            });
    }

    handleSubmit(event) {
        event.preventDefault();

        var val = new Array();

        this.state.boardList.map(board => {
            board.visibility = document.forms['boardlist'].elements[board.id + "visibility"].checked;
            if (board.visibility == true) { val.push(board); }
            board.timeShown = parseInt(document.forms['boardlist'].elements[board.id + "timeShown"].value);
            board.refreshRate = parseInt(document.forms['boardlist'].elements[board.id + "refreshRate"].value);
        })

        this.setState({
            boardPresentation: {
                title: document.forms['presentation'].elements["title"].value,
                owner: document.forms['presentation'].elements["username"].value,
                credentials: {
                    username: document.forms['presentation'].elements["username"].value,
                    password: document.forms['presentation'].elements["password"].value
                },
                boards: {
                    values: val,
                }
            }
        }, this.postPresentation)

    }

    postPresentation() {

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
        if (sessionStorage.getItem('JwtToken') === null) {
            return null;
        }
        let contents = this.state.loading
            ? null
            : BoardList.renderBoardList(this.state.boardList, this.handleSubmit);

        let error = this.state.invalidCredentials
            ? <h4>Nekorekts lietotājvārds un/vai parole!</h4>
            : null

        return <div style={stylePage}>
            <h1>Izveidot prezentāciju</h1>

            <form name="presentation" onSubmit={this.handleForm}>
                <div key="title" style={styleForm}>
                    Nosaukums: <input id="title" required name="title" type="text" />
                </div>
                <div key="username" style={styleForm}>
                    Lietotājvārds: <input id="username" required name="username" type="text" />
                </div>
                <div key="password" style={styleForm}>
                    Parole: <input id="password" required name="password" type="password" />
                </div>
                <div style={styleButton}><button type="submit" className="btn btn-default">Apstiprināt</button></div>
            </form>
                {error}
            {contents}
        </div>;
    }

    private static renderBoardList(boardList: Value[], handleSubmit) {

        return <div>
            <form name='boardlist' onSubmit={handleSubmit}>
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
            <button className="btn btn-default" type="submit">Pievienot prezentāciju</button>
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

const stylePage = {
    marginTop:'70px'
}

interface BoardPresentation {
    title: string;
    owner: string;
    credentials: Credentials;
    boards: FullBoardList;
}