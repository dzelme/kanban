import * as React from 'react';
import 'isomorphic-fetch';
import jwt_decode from 'jwt-decode';
import { RouteComponentProps } from 'react-router';
import { EditPresentationState, Value } from './Interfaces';
import { ApiClient } from './ApiClient';

export class EditPresentation extends React.Component<RouteComponentProps<{ id: string }>, EditPresentationState> {
    constructor(props: RouteComponentProps<{ id: string }>) {
        super(props);

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
            credentials: { username: "", password: "" },
            loading: true,
            authenticated: true
        };

        this.handleFetch = this.handleFetch.bind(this);
        this.handleVisibility = this.handleVisibility.bind(this);
        this.handleForm = this.handleForm.bind(this);
        this.handleAuth = this.handleAuth.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.postPresentation = this.postPresentation.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleChangeBoardVisibility = this.handleChangeBoardVisibility.bind(this);
        this.handleChangeBoardTimes = this.handleChangeBoardTimes.bind(this);

        ApiClient.getSinglePresentation(this.props.match.params.id)
            .then(data => {
                this.setState({ boardPresentation: data, credentials: data.credentials }, this.handleFetch);
            });
    }

    handleForm(event) {
        event.preventDefault();

        this.handleAuth();
    }

    handleAuth() {
        ApiClient.checkCredentials(this.state.credentials)
            .then(response => {
                if (response.status == 200) {
                    this.setState({ authenticated: true }, this.handleFetch);
                }
                else {
                    this.setState({ authenticated: false, loading: true });
                }
            });
    }

    handleFetch() {
        ApiClient.boardList(this.state.credentials)
            .then(data => {
                this.setState({ boardPresentation: this.state.boardPresentation, boardList: data }, this.handleVisibility);
            });
    }

    handleVisibility() {
        var newList = EditPresentation.checkVisibility(this.state.boardList, this.state.boardPresentation.boards.values);
        this.setState({ boardList: newList, loading: false });
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
                owner: this.state.boardPresentation.owner,
                credentials: this.state.credentials,
                boards: {
                    values: val,
                }
            }
        }, this.postPresentation)

    }

    postPresentation() {
        ApiClient.saveUserSettings(this.state.boardList, this.state.boardPresentation.credentials.username);
        ApiClient.savePresentation(this.state.boardPresentation)
            .then(() => open('./admin/presentations', '_self'));
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
        var newBoardList = this.state.boardList;
        var value = parseInt(e.target.value);

        if (name == 'timeShown') {

            this.state.boardList.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardList[index].timeShown = value

                    this.setState({
                        boardList: newBoardList
                    });
                }
            })
        }
        else {
            this.state.boardList.map((board, index) => {

                if (board.id.toString() == id) {
                    newBoardList[index].refreshRate = value

                    this.setState({
                        boardList: newBoardList
                    });
                }
            })
        }
    }

    public render() {
        if (sessionStorage.getItem('JwtToken') === null) {
            return null;
        }

        let contents = this.state.loading
            ? null
            : EditPresentation.renderBoardList(this.state.boardList, this.handleSubmit, this.handleChangeBoardVisibility, this.handleChangeBoardTimes);

        let error = this.state.authenticated
            ? <h4>Brīdinājums! Lietotājvārds un parole tiks glabāti atklātā tekstā uz servera!</h4>
            : <h4>Nekorekts lietotājvārds un/vai parole!</h4>


        return <div className="top-padding">

            <h1>Rediģēt prezentāciju</h1>

            <form name="presentation" onSubmit={this.handleForm}>
                <div key="title" className="FormElement">
                    Nosaukums: <input id="title" name="title" type="text" value={this.state.boardPresentation.title} onChange={this.handleChange} />
                </div>
                <div key="owner" className="FormElement">
                    Izveidotājs: <input id="owner" name="owner" type="text" disabled value={this.state.boardPresentation.owner} />
                </div>
                <div key="username" className="FormElement">
                    Lietotājvārds: <input id="username" required name="username" type="text" value={this.state.credentials.username} onChange={this.handleChange} />
                </div>
                <div key="password" className="FormElement">
                    Parole: <input id="password" required name="password" type="password" value={this.state.credentials.password} onChange={this.handleChange} />
                </div>
                <div className="FormButton"><button type="submit" className="btn btn-default">Apstiprināt izmaiņas <br /> autentifikācijas datos</button></div>
            </form>

            {error}
            {contents}
        </div>;
    }

    private static checkVisibility(allBoards: Value[], presentationBoards: Value[]) {

        presentationBoards.map(presBoard =>
            allBoards.map(board => {
                if (board.id == presBoard.id) {
                    board.visibility = true,
                        board.timeShown = presBoard.timeShown,
                        board.refreshRate = presBoard.refreshRate
                }
            })
        )
        return allBoards;
    }

    private static renderBoardList(boardList: Value[], handleSubmit, handleChangeBoardVisibility, handleChangeBoardTimes) {

        return <div>
            <form name='boardlist' onSubmit={handleSubmit}>
                <table className='table'>
                    <thead>
                        <tr>
                            <th>ID</th>
                            <th>Nosaukums</th>
                            <th className="CheckBox">Iekļaut prezentācijā</th>
                            <th>Attēlošanas laiks(s)</th>
                            <th>Atjaunošanas laiks(s)</th>
                        </tr>
                    </thead>
                    <tbody>
                        {boardList.map(board =>
                            <tr key={board.id + "row"}>
                                <td key={board.id + ""}>{board.id}</td>
                                <td key={board.id + "name"}>{board.name}</td>
                                <td key={board.id + "visibility"} className="CheckBox"><input name={board.id + "visibility"} type="checkbox" defaultChecked={board.visibility} onClick={() => handleChangeBoardVisibility(board.id)} /></td>
                                <td key={board.id + "timeShown"}><input name={board.id + "timeShown"} type="number" value={board.timeShown.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'timeShown', e)} /></td>
                                <td key={board.id + "refreshRate"}><input name={board.id + "refreshRate"} type="number" value={board.refreshRate.toString()} onChange={(e) => handleChangeBoardTimes(board.id, 'refreshRate', e)} /></td>
                            </tr>
                        )}
                    </tbody>
                </table>
                <button className="btn btn-default" type="submit">Saglabāt izmaiņas</button>
            </form>
        </div>
    }
}