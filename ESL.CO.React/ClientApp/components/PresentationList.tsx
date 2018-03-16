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

        fetch('api/SampleData/PresentationList', {
            headers: {
                authorization: 'Bearer ' + sessionStorage.getItem('JwtToken')
            }
        })
            .then(response => response.json() as Promise<BoardPresentation[]>)
            .then(data => {
                this.setState({ presentationList: data, loading: false });
            });
    }

    public render() {

        let contents = this.state.loading
            ? <p><em>Loading...</em></p>
            : PresentationList.renderBoardList(this.state.presentationList);

        return <div>

            {contents}
        </div>;
    }

    private static renderBoardList(boardlist: BoardPresentation[]) {
        return <div>
            <h1>Board List</h1>
            <table className='table'>
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Title</th>
                        <th>Owner</th>
                        <th>Visibility</th>
                        <th>Boards</th>
                    </tr>
                </thead>

            </table>
        </div>
    }


}