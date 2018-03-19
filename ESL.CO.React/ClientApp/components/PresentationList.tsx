import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import 'isomorphic-fetch';
import { BoardPresentation } from './Interfaces';

interface PresentationState {
    presentationList: BoardPresentation[];
    loading: boolean;
}

export class PresentationList extends React.Component<RouteComponentProps<{}>, PresentationState> {
    constructor() {
        super();

        this.state = { presentationList: [], loading: true };


        function handleErrors(response) {
            if (response.status == 401) {
                open('/login','_self');
            }           
            else if (!response.ok) {
                throw Error(response.statusText);
            }
            return response;
        }

        fetch('api/presentations', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(handleErrors)
            .then(response => response.json())
            .then(data => {
                this.setState({ presentationList: data, loading: false });
            });
    }

    public render() {

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : PresentationList.renderPresentationList(this.state.presentationList);

        return <div>
            {contents}
        </div>;
    }

    private static renderPresentationList(presentationList: BoardPresentation[]) {
        return <div>
            <h1>Prezentāciju saraksts</h1>
            <table className='table'>
                <thead style={styleHeader}>
                    <tr>
                        <th>ID</th>
                        <th>Nosaukums</th>
                        <th>Izveidotājs</th>
                        <th>Paneļi</th>
                    </tr>
                </thead>
                <tbody style={styleContent}>
                    {presentationList.map(presentation =>
                        <tr key={presentation.id + "row"}>
                            <td key={presentation.id + ""}>{presentation.id}</td>
                            <td key={presentation.id + "title"}>{presentation.title}</td>
                            <td key={presentation.id + "owner"}>{presentation.owner}</td>
                            <td key={presentation.id + "boards"}>
                                {presentation.boards.values.map(board =>
                                    board.id + " " 
                                )}
                            </td>
                        </tr>
                    )}
                </tbody>
            </table>
        </div>
    }
}

const styleHeader = {
    fontSize: '20px'
}

const styleContent = {
    fontSize: '15px'
}