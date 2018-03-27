import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { Credentials, Value, BoardPresentation } from './Interfaces';
import jwt_decode from 'jwt-decode';
import { ApiClient } from './ApiClient';

interface BoardListState {
    boardPresentation: BoardPresentation;
    boardList: Value[];
    loading: boolean;
    credentials: Credentials;
    authenticated: boolean;
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
            loading: true,
            credentials: { username: "", password: "" },
            authenticated: true
        };

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleAuth = this.handleAuth.bind(this);
        this.handleFetch = this.handleFetch.bind(this);
        this.postPresentation = this.postPresentation.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleChangeBoardVisibility = this.handleChangeBoardVisibility.bind(this);
        this.handleChangeBoardTimes = this.handleChangeBoardTimes.bind(this);

    }

    handleAuth(event) {
        event.preventDefault();
        ApiClient.login(this.state.credentials)
            .then(response => {
                if (response) {
                    this.setState({ authenticated: true }, this.handleFetch);
                }
                else {
                    this.setState({ authenticated: false });
                }
            });
    }

    handleFetch() {
        ApiClient.boardList(this.state.credentials)
            .then(data => {
                this.setState({ boardList: data, loading: false });
            });
    }

    handleSubmit(event) {
        event.preventDefault();

        var val = new Array();

        this.state.boardList.map(board => {
            if (board.visibility == true) { val.push(board); }
        })

        this.setState({
            boardPresentation: {
                id: this.state.boardPresentation.id,
                title: this.state.boardPresentation.title,
                owner: jwt_decode(sessionStorage.getItem('JwtToken'))['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'],
                credentials: this.state.credentials,
                boards: {
                    values: val,
                }
            }
        }, this.postPresentation)
    }

    postPresentation() {

        ApiClient.savePresentation(this.state.boardPresentation);
        open('./admin/presentations', '_self');
    }

    handleChange(event) {
        const target = event.target;
        const name = target.name;

        if (name == 'title') {
            this.setState({ boardPresentation: { id: this.state.boardPresentation.id, title: event.target.value, owner: this.state.boardPresentation.owner, credentials: this.state.boardPresentation.credentials, boards: this.state.boardPresentation.boards } });
        }
        else if (name == 'username') {
            this.setState({ credentials: { username: event.target.value, password: this.state.credentials.password } });
        }
        else {
            this.setState({ credentials: { username: this.state.credentials.username, password: event.target.value } });
        }
    }

    handleChangeBoardVisibility(id: string) {
        var newBoardlist = this.state.boardList;

        this.state.boardList.map((board, index) => {
            if (board.id.toString() == id) {
                newBoardlist[index].visibility = !newBoardlist[index].visibility

                this.setState({
                    boardList: newBoardlist
                });
            }
        })
    }

    handleChangeBoardTimes(id: string, name: string, e) {
        var newBoardlist = this.state.boardList;
        var value = parseInt(e.target.value);

        if (name == 'timeShown') {

            this.state.boardList.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardlist[index].timeShown = value

                    this.setState({
                        boardList: newBoardlist
                    });
                }
            })
        }
        else {
            this.state.boardList.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardlist[index].refreshRate = value

                    this.setState({
                        boardList: newBoardlist
                    });
                }
            })
        }
    }


    // used to redirect to login screen, if invalid JWT token
    componentWillMount() {
        ApiClient.hasValidJwt()
            .then(response => ApiClient.redirect(response, 401, './login'));
    }

    public render() {
        if (sessionStorage.getItem('JwtToken') === null) {
            return null;
        }

        let contents = this.state.loading
            ? null
            : BoardList.renderBoardList(this.state.boardList, this.handleSubmit, this.handleChangeBoardVisibility, this.handleChangeBoardTimes);

        let error = this.state.authenticated
            ? <h4>Brīdinājums! Lietotājvārds un parole tiks glabāti atklātā tekstā uz servera!</h4>
            : <h4>Nekorekts lietotājvārds un/vai parole!</h4>

        return <div className="top-padding">
            <h1>Izveidot prezentāciju</h1>

            <form name="presentation" onSubmit={this.handleAuth}>
                <div key="title" style={styleForm}>
                    Nosaukums: <input id="title" required name="title" type="text" value={this.state.boardPresentation.title} onChange={this.handleChange} />
                </div>
                <div key="username" style={styleForm}>
                    Lietotājvārds: <input id="username" required name="username" type="text" value={this.state.credentials.username} onChange={this.handleChange}/>
                </div>
                <div key="password" style={styleForm}>
                    Parole: <input id="password" required name="password" type="password" value={this.state.credentials.password} onChange={this.handleChange}/>
                </div>
                <div style={styleButton}><button type="submit" className="btn btn-default">Apstiprināt</button></div>
            </form>
            {error}
            {contents}
        </div>;
    }

    private static renderBoardList(boardList: Value[], handleSubmit, handleChangeBoardVisibility, handleChangeBoardTimes) {

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
                                <td key={board.id + "visibility"}><input name={board.id + "visibility"} type="checkbox" defaultChecked={board.visibility} onClick={() => handleChangeBoardVisibility(board.id)}/></td>
                                <td key={board.id + "timeShown"}><input name={board.id + "timeShown"} type="number" value={board.timeShown.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'timeShown', e)}/></td>
                                <td key={board.id + "refreshRate"}><input name={board.id + "refreshRate"} type="number" value={board.refreshRate.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'refreshRate', e)}/></td>
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
    height: '40px'
}