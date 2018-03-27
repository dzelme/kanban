import * as React from 'react';

export default class BoardName extends React.Component<{ name: string, fromCache: boolean, message: string, allNames: string[] }> {

    public render() {

        var nameCount = this.props.allNames.length;
        var size = 100 / nameCount;
        styleList.width = size + '%';
        styleActive.width = size + '%';

        return <div>
            {
                this.props.allNames.map((name, index) =>
                    <div style={BoardName.statusColor(this.props.name, name)} key={index}> <h3>{name}</h3></div>
                    )
            }
            {this.props.fromCache ? <h4>Dati no keša</h4> : ""}<h4>{this.props.message}</h4>
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

const styleList = {
    display: 'inline-block',
    width: '',
    border: 'solid',
    borderColor: 'white',
    textAlign: 'center',
    marginBottom:'30px'
}

const styleActive = {
    display: 'inline-block',
    width: '',
    border: 'solid',
    borderColor: 'white',
    textAlign: 'center',
    background: 'grey',
    color: 'white',
    marginBottom: '30px'
}