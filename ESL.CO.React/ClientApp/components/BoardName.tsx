import * as React from 'react';
import { Link } from 'react-router-dom';
import { Value } from './Interfaces';


const styleList = {
    width: '',
    background:'#000'
}

const styleActive = {
    width: '',
    background: '#333',
    color: 'white',
}

export default class BoardName extends React.Component<{ presentationId: string, name: string, fromCache: boolean, message: string, boardlist: Value[] }> {

    public render() {

        var nameCount = this.props.boardlist.length;
        var size = 100 / nameCount;
        styleList.width = size + '%';
        styleActive.width = size + '%';

        return <div className="BoardNames">
            {
                this.props.boardlist.map((board, index) =>
                    <div style={BoardName.statusColor(this.props.name, board.name)} key={index} className="NamesBar"><Link to={"/k/" + this.props.presentationId + "/" + board.id}> <h3>{board.name}</h3>{this.props.fromCache ? <h4>Dati no keša</h4> : ""}<h4>{this.props.message}</h4></Link></div>
                    )
            }
        </div>
    }
    private static statusColor(currentBoardName: string, boardName: string) {
        let style;

        if (boardName == currentBoardName) {
            style = styleActive;
        }
        else {
            style = styleList;
        }

        return style;
    }
}